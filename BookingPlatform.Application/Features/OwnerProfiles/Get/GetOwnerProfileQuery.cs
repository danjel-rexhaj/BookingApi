using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.OwnerProfiles.Get
{
    public record GetOwnerProfileQuery() : IRequest<OwnerProfileDto>;
}
