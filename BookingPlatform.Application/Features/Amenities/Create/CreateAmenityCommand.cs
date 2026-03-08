using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace BookingPlatform.Application.Features.Amenities.Create
{
    public record CreateAmenityCommand(string Name) : IRequest<Guid>;
}