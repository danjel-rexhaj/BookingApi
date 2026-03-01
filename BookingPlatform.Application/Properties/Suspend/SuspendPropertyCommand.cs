using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Properties.Suspend;

public record SuspendPropertyCommand(Guid PropertyId) : IRequest;