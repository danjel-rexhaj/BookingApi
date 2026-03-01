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
    string? PropertyType,
    DateTime? StartDate,
    DateTime? EndDate,
    string? SortBy,
    int Page = 1,
    int PageSize = 10
) : IRequest<List<PropertyDto>>;