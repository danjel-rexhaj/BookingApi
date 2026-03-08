using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BookingPlatform.Application.Features.Properties.Update
{
    public record UpdatePropertyAmenitiesCommand(
        Guid PropertyId,
        List<Guid> AmenityIds
    ) : IRequest<Unit>;
}
