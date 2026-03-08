using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Properties.Update
{
    public record UpdatePropertyCommand(
        Guid PropertyId,
        string Name,
        string Description,
        string PropertyType,
        int MaxGuests,
        decimal BasePricePerNight,
        TimeSpan CheckInTime,
        TimeSpan CheckOutTime
    ) : IRequest<Unit>;
}
