using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookingPlatform.Application.Features.Properties.Search;

public record PropertyDto(
    Guid Id,
    string Country,
    string City,
    string Name,
    string Description,
    int MaxGuests,
    string PropertyType,
    decimal price,
    double AverageRating,
    int TotalReviews
);
