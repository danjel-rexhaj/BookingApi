using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System;
using System.Collections.Generic;

namespace BookingPlatform.Application.Features.Admin.Bookings.GetAllBookings;

public record GetAllBookingsQuery()
    : IRequest<List<GetAllBookingsResponse>>;
