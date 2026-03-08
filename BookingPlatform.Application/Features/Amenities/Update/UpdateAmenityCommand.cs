using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Features.Amenities.Update
{
    public record UpdateAmenityCommand(
        Guid Id,
        string Name
    ) : IRequest<Unit>;
}