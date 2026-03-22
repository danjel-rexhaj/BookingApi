using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using Hangfire;
using MediatR;
using System.Text.Json;
using BookingPlatform.Application.Events;

namespace BookingPlatform.Application.Features.Bookings.Cancel;

public class CancelBookingHandler : IRequestHandler<CancelBookingCommand, Unit>
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IPropertyRepository _propertyRepo;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IOutboxRepository _outboxRepository;

    public CancelBookingHandler(
        IBookingRepository bookingRepo,
        IPropertyRepository propertyRepo,
        IUserRepository userRepository,
        ICurrentUserService currentUser,
        INotificationRepository notificationRepository,
        INotificationService notificationService,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        IOutboxRepository outboxRepository)
    {
        _bookingRepo = bookingRepo;
        _propertyRepo = propertyRepo;
        _userRepository = userRepository;
        _currentUser = currentUser;
        _notificationRepository = notificationRepository;
        _notificationService = notificationService;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _outboxRepository = outboxRepository;
    }

    public async Task<Unit> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null)
            throw new Exception("Booking not found.");

        if (booking.GuestId != _currentUser.UserId)
            throw new Exception("You are not allowed to cancel this booking.");

        var guest = await _userRepository.GetByIdAsync(booking.GuestId);
        if (guest == null)
            throw new Exception("Guest not found.");

        var property = await _propertyRepo.GetByIdAsync(booking.PropertyId);
        if (property == null)
            throw new Exception("Property not found.");

        var owner = await _userRepository.GetByIdAsync(property.OwnerId);
        if (owner == null)
            throw new Exception("Owner not found.");

        var canceledAtUtc = DateTime.UtcNow;
        booking.CancelWithPolicy();

        var guestMessage = "Your booking has been canceled.";
        var ownerMessage = "A booking has been canceled by the guest.";

        var guestNotification = new Notification(
            booking.GuestId,
            guestMessage,
            "BookingCanceled"
        );

        var ownerNotification = new Notification(
            property.OwnerId,
            ownerMessage,
            "BookingCanceled"
        );

        await _notificationRepository.AddAsync(guestNotification);
        await _notificationRepository.AddAsync(ownerNotification);

        await _bookingRepo.SaveChangesAsync();

        var bookingCanceledEvent = new BookingCanceledIntegrationEvent
        {
            BookingId = booking.Id,
            GuestId = booking.GuestId,
            PropertyId = booking.PropertyId,
            CheckInDate = booking.StartDate,
            CheckOutDate = booking.EndDate,
            CanceledAtUtc = canceledAtUtc
        };

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(BookingCanceledIntegrationEvent),
            Payload = JsonSerializer.Serialize(bookingCanceledEvent),
            OccurredOnUtc = DateTime.UtcNow
        };

        await _outboxRepository.AddAsync(outboxMessage);
        await _outboxRepository.SaveChangesAsync();

        await _notificationService.SendNotificationAsync(
            booking.GuestId,
            guestMessage
        );

        await _notificationService.SendNotificationAsync(
            property.OwnerId,
            ownerMessage
        );

        var guestEmailBody = _emailTemplateService.GetBookingCancelledEmail(
            guest.FirstName,
            property.Name,
            booking.StartDate,
            booking.EndDate
        );

        BackgroundJob.Enqueue(() =>
            _emailService.SendEmailAsync(
                guest.Email,
                "Your booking has been canceled",
                guestEmailBody
            )
        );

        var ownerEmailBody = $"The booking for property {property.Name} has been canceled by the guest.";

        BackgroundJob.Enqueue(() =>
            _emailService.SendEmailAsync(
                owner.Email,
                "Booking canceled by guest",
                ownerEmailBody
            )
        );

        return Unit.Value;
    }
}