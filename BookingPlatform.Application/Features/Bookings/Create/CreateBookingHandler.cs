using System.Text.Json;
using BookingPlatform.Application.Events;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Domain.Enums;
using Hangfire;
using MediatR;

namespace BookingPlatform.Application.Features.Bookings.Create;

public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBlockedDateRepository _blockedDateRepository;
    private readonly ISeasonalPriceRepository _seasonalPriceRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;

    public CreateBookingHandler(
        IBookingRepository bookingRepository,
        IPropertyRepository propertyRepository,
        ICurrentUserService currentUserService,
        IBlockedDateRepository blockedDateRepository,
        ISeasonalPriceRepository seasonalPriceRepository,
        IOutboxRepository outboxRepository,
        IUserRepository userRepository,
        INotificationRepository notificationRepository,
        INotificationService notificationService,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService)
    {
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
        _blockedDateRepository = blockedDateRepository;
        _seasonalPriceRepository = seasonalPriceRepository;
        _outboxRepository = outboxRepository;
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
        _notificationService = notificationService;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
    }

    public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);

        if (property == null || !property.IsApproved || !property.IsActive)
            throw new Exception("Property not available");

        if (request.GuestCount > property.MaxGuests)
            throw new Exception("Guest limit exceeded");

        if (request.StartDate >= request.EndDate)
            throw new Exception("Invalid date range");

        var existingBookings = await _bookingRepository.GetBookingsForPropertyAsync(request.PropertyId);

        foreach (var booking in existingBookings)
        {
            if (booking.BookingStatus != BookingStatus.Confirmed)
                continue;

            bool overlap =
                request.StartDate < booking.EndDate &&
                request.EndDate > booking.StartDate;

            if (overlap)
                throw new Exception("Property already booked for selected dates");
        }

        var nights = (request.EndDate - request.StartDate).Days;

        if (nights <= 0)
            throw new Exception("Stay must be at least 1 night");

        if (property.MinimumStay.HasValue && nights < property.MinimumStay.Value)
            throw new Exception($"Minimum stay is {property.MinimumStay.Value} nights");

        if (property.MaximumStay.HasValue && nights > property.MaximumStay.Value)
            throw new Exception($"Maximum stay is {property.MaximumStay.Value} nights");

        var blockedDates = await _blockedDateRepository.GetByPropertyIdAsync(request.PropertyId);

        foreach (var blocked in blockedDates)
        {
            if (blocked.Date >= request.StartDate.Date &&
                blocked.Date < request.EndDate.Date)
            {
                throw new Exception("Selected dates include blocked dates");
            }
        }

        decimal basePricePerNight = property.BasePricePerNight;
        var seasonalPrices = await _seasonalPriceRepository.GetByPropertyIdAsync(request.PropertyId);

        decimal priceForPeriod = 0;

        for (var date = request.StartDate.Date; date < request.EndDate.Date; date = date.AddDays(1))
        {
            var seasonal = seasonalPrices.FirstOrDefault(sp =>
                date >= sp.StartDate.Date &&
                date <= sp.EndDate.Date);

            if (seasonal != null)
                priceForPeriod += seasonal.PricePerNight;
            else
                priceForPeriod += basePricePerNight;
        }

        int extraGuests = request.GuestCount > 1 ? request.GuestCount - 1 : 0;
        decimal amenitiesUpCharge = extraGuests * 5m * nights;
        decimal cleaningFee = 20m;

        decimal subtotal = priceForPeriod + amenitiesUpCharge + cleaningFee;
        decimal subtotalAfterDiscount = subtotal;
        decimal taxAmount = subtotalAfterDiscount * 0.10m;
        decimal totalPrice = subtotalAfterDiscount + taxAmount;

        var newBooking = new Booking(
            request.PropertyId,
            _currentUserService.UserId,
            request.StartDate,
            request.EndDate,
            request.GuestCount
        );

        newBooking.SetPricing(
            cleaningFee,
            amenitiesUpCharge,
            priceForPeriod,
            taxAmount,
            totalPrice
        );

        await _bookingRepository.AddAsync(newBooking);

        var ownerNotificationMessage = "You have received a new booking request.";
        var ownerNotification = new Notification(
            property.OwnerId,
            ownerNotificationMessage,
            "BookingCreated"
        );

        var guestNotificationMessage = "Your booking request has been created successfully.";
        var guestNotification = new Notification(
            newBooking.GuestId,
            guestNotificationMessage,
            "BookingCreated"
        );

        await _notificationRepository.AddAsync(ownerNotification);
        await _notificationRepository.AddAsync(guestNotification);
        await _bookingRepository.SaveChangesAsync();

        try
        {
            var bookingCreatedEvent = new BookingCreatedIntegrationEvent
            {
                BookingId = newBooking.Id,
                GuestId = newBooking.GuestId,
                PropertyId = newBooking.PropertyId,
                CheckInDate = newBooking.StartDate,
                CheckOutDate = newBooking.EndDate,
                TotalPrice = totalPrice
            };

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = nameof(BookingCreatedIntegrationEvent),
                Payload = JsonSerializer.Serialize(bookingCreatedEvent),
                OccurredOnUtc = DateTime.UtcNow
            };

            await _outboxRepository.AddAsync(outboxMessage);
            await _outboxRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.InnerException?.Message ?? ex.Message);
        }

        await _notificationService.SendNotificationAsync(
            property.OwnerId,
            ownerNotificationMessage
        );

        await _notificationService.SendNotificationAsync(
            newBooking.GuestId,
            guestNotificationMessage
        );

        var owner = await _userRepository.GetByIdAsync(property.OwnerId);
        if (owner != null)
        {
            var ownerEmailBody = _emailTemplateService.GetNewBookingRequestEmail(
                owner.FirstName,
                property.Name,
                newBooking.StartDate,
                newBooking.EndDate,
                newBooking.GuestCount,
                newBooking.TotalPrice
            );

            BackgroundJob.Enqueue(() =>
                _emailService.SendEmailAsync(
                    owner.Email,
                    "New booking request for your property",
                    ownerEmailBody
                )
            );
        }

        var guest = await _userRepository.GetByIdAsync(newBooking.GuestId);
        if (guest != null)
        {
            var guestEmailBody = _emailTemplateService.GetBookingCreatedEmail(
                 guest.FirstName,
                 property.Name,
                 newBooking.StartDate,
                 newBooking.EndDate,
                 newBooking.GuestCount,
                 newBooking.TotalPrice
             );

            BackgroundJob.Enqueue(() =>
                _emailService.SendEmailAsync(
                    guest.Email,
                    "Your booking request has been created",
                    guestEmailBody
                )
            );
        }

        return newBooking.Id;
    }
}