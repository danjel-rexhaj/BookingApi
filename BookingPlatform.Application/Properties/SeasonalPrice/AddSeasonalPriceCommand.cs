using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Features.Properties.SeasonalPrice;

public record AddSeasonalPriceCommand(
    Guid PropertyId,
    DateTime StartDate,
    DateTime EndDate,
    decimal PricePerNight
) : IRequest<Guid>;