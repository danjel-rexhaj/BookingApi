using BookingPlatform.Application.Features.Bookings.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Features.Bookings.Create;

public record CreateBookingCommand(
    Guid PropertyId,
    DateTime StartDate,
    DateTime EndDate,
    int GuestCount,
    decimal PricePerNight,
    decimal CleaningFee,
    decimal AmenitiesUpCharge
) : IRequest<Guid>;
