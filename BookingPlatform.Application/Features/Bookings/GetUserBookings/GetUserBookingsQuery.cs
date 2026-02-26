using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Features.Bookings.GetUserBookings;

public record GetUserBookingsQuery(Guid UserId)
    : IRequest<List<GetUserBookingsResponse>>;
