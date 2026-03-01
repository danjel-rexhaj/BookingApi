using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Domain.Enums;
using MediatR;

namespace BookingPlatform.Application.Features.Bookings.Create;

public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBlockedDateRepository _blockedDateRepository;
    private readonly ISeasonalPriceRepository _seasonalPriceRepository;
    public CreateBookingHandler(
        IBookingRepository bookingRepository,
        IPropertyRepository propertyRepository,
        ICurrentUserService currentUserService,
        IBlockedDateRepository blockedDateRepository,
        ISeasonalPriceRepository seasonalPriceRepository)
    {
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
        _blockedDateRepository = blockedDateRepository;
        _seasonalPriceRepository = seasonalPriceRepository;
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

        var existingBookings =
            await _bookingRepository.GetBookingsForPropertyAsync(request.PropertyId);

        foreach (var booking in existingBookings)
        {
            if (booking.BookingStatus != BookingStatus.Confirmed)
                continue;

            var overlap =
                request.StartDate < booking.EndDate &&
                request.EndDate > booking.StartDate;

            if (overlap)
                throw new Exception("Property already booked for selected dates");
        }


        // Nights
        var nights = (request.EndDate - request.StartDate).Days;
        if (nights <= 0)
            throw new Exception("Stay must be at least 1 night");

        //  Minimum / Maximum stay validation
        if (property.MinimumStay.HasValue && nights < property.MinimumStay.Value)
            throw new Exception($"Minimum stay is {property.MinimumStay.Value} nights");

        if (property.MaximumStay.HasValue && nights > property.MaximumStay.Value)
            throw new Exception($"Maximum stay is {property.MaximumStay.Value} nights");

        //  Check Blocked Dates
        var blockedDates = await _blockedDateRepository
            .GetByPropertyIdAsync(request.PropertyId);

        foreach (var blocked in blockedDates)
        {
            if (blocked.Date >= request.StartDate.Date &&
                blocked.Date < request.EndDate.Date)
            {
                throw new Exception("Selected dates include blocked dates");
            }
        }

        //  Base price default
        decimal basePricePerNight = property.PropertyType switch
        {
            "Apartment" => 50m,
            "Hotel" => 80m,
            _ => 60m
        };

        // Seasonal pricing override
        var seasonalPrices = await _seasonalPriceRepository
            .GetByPropertyIdAsync(request.PropertyId);

        decimal priceForPeriod = 0;

        for (var date = request.StartDate.Date;
             date < request.EndDate.Date;
             date = date.AddDays(1))
        {
            var seasonal = seasonalPrices
                .FirstOrDefault(sp =>
                    date >= sp.StartDate.Date &&
                    date <= sp.EndDate.Date);

            if (seasonal != null)
                priceForPeriod += seasonal.PricePerNight;
            else
                priceForPeriod += basePricePerNight;
        }

        //  Additional guest fee
        int extraGuests = request.GuestCount > 1 ? request.GuestCount - 1 : 0;
        decimal amenitiesUpCharge = extraGuests * 15m * nights;

        //  Cleaning fee
        decimal cleaningFee = 20m;

        //  Subtotal
        decimal subtotal = priceForPeriod + amenitiesUpCharge + cleaningFee;

        //  Discount logic
        if (property.DiscountPercentage.HasValue &&
            property.DiscountValidFrom.HasValue &&
            property.DiscountValidTo.HasValue)
        {
            var today = DateTime.UtcNow.Date;

            if (today >= property.DiscountValidFrom.Value &&
                today <= property.DiscountValidTo.Value)
            {
                var discountAmount = subtotal *
                    (property.DiscountPercentage.Value / 100m);

                subtotal -= discountAmount;
            }
        }

        //  Final total
        decimal totalPrice = subtotal;

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
            totalPrice
        );

        await _bookingRepository.AddAsync(newBooking);
        await _bookingRepository.SaveChangesAsync();

        return newBooking.Id;
    }
}