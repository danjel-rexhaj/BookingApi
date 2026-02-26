using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Features.Bookings.Cancel;

public record CancelBookingCommand(Guid BookingId) : IRequest<Unit>;