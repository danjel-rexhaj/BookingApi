using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Features.Bookings.GetUserBookings;

using BookingPlatform.Domain.Enums;
using MediatR;

public record GetUserBookingsQuery(
    Guid UserId,
    BookingStatus? Status
) : IRequest<List<GetUserBookingsResponse>>;
