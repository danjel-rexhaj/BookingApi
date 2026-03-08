using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Features.Properties.Search;

public record GetPropertiesQuery(
    string? Country,
    string? City,
    int? Guests,
    string? PropertyType,
    string? Amaneties,
    string? Rating,
    string? Price,
    DateTime? StartDate,
    DateTime? EndDate,
    string? SortBy
) : IRequest<List<PropertyDto>>;