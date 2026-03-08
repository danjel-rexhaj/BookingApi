using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Properties.Reject
{
    public record RejectPropertyCommand(Guid PropertyId) : IRequest;
}
