using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Features.Properties.BlockDate;

public record AddBlockedDateCommand(
    Guid PropertyId,
    DateTime Date
) : IRequest<Guid>;
