using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Bookings.Create;

public class CreateBookingHandler
    : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateBookingHandler(
        IBookingRepository bookingRepository,
        IPropertyRepository propertyRepository,
        ICurrentUserService currentUserService)
    {
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(
        CreateBookingCommand request,
        CancellationToken cancellationToken)
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
            var overlap =
                request.StartDate < booking.EndDate &&
                request.EndDate > booking.StartDate;

            if (overlap)
                throw new Exception("Property already booked for selected dates");
        }

        var nights = (request.EndDate - request.StartDate).Days;

        var priceForPeriod = nights * request.PricePerNight;
        var totalPrice = priceForPeriod +
                         request.CleaningFee +
                         request.AmenitiesUpCharge;

        var newBooking = new Booking(
            request.PropertyId,
            _currentUserService.UserId, // merr nga JWT
            request.StartDate,
            request.EndDate,
            request.GuestCount
        );

        newBooking.SetPricing(
            request.CleaningFee,
            request.AmenitiesUpCharge,
            priceForPeriod,
            totalPrice
        );

        await _bookingRepository.AddAsync(newBooking);
        await _bookingRepository.SaveChangesAsync();

        return newBooking.Id;
    }
}
