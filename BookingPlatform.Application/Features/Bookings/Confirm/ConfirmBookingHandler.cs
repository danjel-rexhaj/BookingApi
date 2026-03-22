using BookingPlatform.Application.Events;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using Hangfire;
using MediatR;
using System.Text.Json;

namespace BookingPlatform.Application.Features.Bookings.Confirm;

public class ConfirmBookingHandler : IRequestHandler<ConfirmBookingCommand, Unit>
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

    public ConfirmBookingHandler(
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

    public async Task<Unit> Handle(
        ConfirmBookingCommand request,
        CancellationToken cancellationToken)
    {
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null)
            throw new Exception("Booking not found.");

        var property = await _propertyRepo.GetByIdAsync(booking.PropertyId);
        if (property == null)
            throw new Exception("Property not found.");

        if (property.OwnerId != _currentUser.UserId)
            throw new Exception("You are not allowed to confirm this booking.");

        var guest = await _userRepository.GetByIdAsync(booking.GuestId);
        if (guest == null)
            throw new Exception("Guest not found.");

        var confirmedAtUtc = DateTime.UtcNow;
        booking.Confirm(confirmedAtUtc);

        var notificationMessage = "Your booking has been confirmed.";

        var notification = new Notification(
            booking.GuestId,
            notificationMessage,
            "BookingConfirmed"
        );

        await _notificationRepository.AddAsync(notification);
        await _bookingRepo.SaveChangesAsync();

        var bookingConfirmedEvent = new BookingConfirmedIntegrationEvent
        {
            BookingId = booking.Id,
            GuestId = booking.GuestId,
            PropertyId = booking.PropertyId,
            CheckInDate = booking.StartDate,
            CheckOutDate = booking.EndDate,
            ConfirmedAtUtc = confirmedAtUtc
        };

        var bookingConfirmedOutbox = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(BookingConfirmedIntegrationEvent),
            Payload = JsonSerializer.Serialize(bookingConfirmedEvent),
            OccurredOnUtc = DateTime.UtcNow
        };

        await _outboxRepository.AddAsync(bookingConfirmedOutbox);

        var reminderScheduledForUtc = booking.StartDate.Date.AddDays(-1);

        var bookingReminderEvent = new BookingReminderScheduledIntegrationEvent
        {
            BookingId = booking.Id,
            GuestId = booking.GuestId,
            PropertyId = booking.PropertyId,
            CheckInDate = booking.StartDate,
            CheckOutDate = booking.EndDate,
            ReminderScheduledForUtc = reminderScheduledForUtc
        };

        var bookingReminderOutbox = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(BookingReminderScheduledIntegrationEvent),
            Payload = JsonSerializer.Serialize(bookingReminderEvent),
            OccurredOnUtc = DateTime.UtcNow
        };

        await _outboxRepository.AddAsync(bookingReminderOutbox);
        await _outboxRepository.SaveChangesAsync();

        await _notificationService.SendNotificationAsync(
            booking.GuestId,
            notificationMessage
        );

        var emailBody = _emailTemplateService.GetBookingConfirmedEmail(
            guest.FirstName,
            property.Name,
            booking.StartDate,
            booking.EndDate,
            booking.TotalPrice
        );

        BackgroundJob.Enqueue(() =>
            _emailService.SendEmailAsync(
                guest.Email,
                "Your booking has been confirmed",
                emailBody
            )
        );

        return Unit.Value;
    }
}