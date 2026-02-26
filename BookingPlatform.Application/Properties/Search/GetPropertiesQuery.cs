using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Properties.Search;

public record GetPropertiesQuery(
    string? City,
    int? Guests,
    string? PropertyType
) : IRequest<List<PropertyDto>>;