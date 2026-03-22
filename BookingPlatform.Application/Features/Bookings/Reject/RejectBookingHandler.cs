using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using Hangfire;
using MediatR;
using System.Text.Json;
using BookingPlatform.Application.Events;

namespace BookingPlatform.Application.Features.Bookings.Reject;

public class RejectBookingHandler : IRequestHandler<RejectBookingCommand, Unit>
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

    public RejectBookingHandler(
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

    public async Task<Unit> Handle(RejectBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null)
            throw new Exception("Booking not found.");

        var property = await _propertyRepo.GetByIdAsync(booking.PropertyId);
        if (property == null)
            throw new Exception("Property not found.");

        if (property.OwnerId != _currentUser.UserId)
            throw new Exception("You are not allowed to reject this booking.");

        var guest = await _userRepository.GetByIdAsync(booking.GuestId);
        if (guest == null)
            throw new Exception("Guest not found.");

        var owner = await _userRepository.GetByIdAsync(property.OwnerId);
        if (owner == null)
            throw new Exception("Owner not found.");

        var rejectedAtUtc = DateTime.UtcNow;
        booking.Reject(rejectedAtUtc);

        var guestMessage = "Your booking has been rejected.";
        var ownerMessage = "You rejected a booking request.";

        var guestNotification = new Notification(
            booking.GuestId,
            guestMessage,
            "BookingRejected"
        );

        var ownerNotification = new Notification(
            property.OwnerId,
            ownerMessage,
            "BookingRejected"
        );

        await _notificationRepository.AddAsync(guestNotification);
        await _notificationRepository.AddAsync(ownerNotification);

        await _bookingRepo.SaveChangesAsync();

        var bookingRejectedEvent = new BookingRejectedIntegrationEvent
        {
            BookingId = booking.Id,
            GuestId = booking.GuestId,
            PropertyId = booking.PropertyId,
            CheckInDate = booking.StartDate,
            CheckOutDate = booking.EndDate,
            RejectedAtUtc = rejectedAtUtc
        };

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(BookingRejectedIntegrationEvent),
            Payload = JsonSerializer.Serialize(bookingRejectedEvent),
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

        var guestEmailBody = _emailTemplateService.GetBookingRejectedEmail(
            guest.FirstName,
            property.Name,
            booking.StartDate,
            booking.EndDate
        );

        BackgroundJob.Enqueue(() =>
            _emailService.SendEmailAsync(
                guest.Email,
                "Your booking has been rejected",
                guestEmailBody
            )
        );

        var ownerEmailBody = $"You rejected the booking request for property {property.Name}.";

        BackgroundJob.Enqueue(() =>
            _emailService.SendEmailAsync(
                owner.Email,
                "Booking request rejected",
                ownerEmailBody
            )
        );

        return Unit.Value;
    }
}