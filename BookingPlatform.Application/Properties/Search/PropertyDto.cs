using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookingPlatform.Application.Properties.Search;

public record PropertyDto(
    Guid Id,
    string Name,
    string Description,
    string City,
    int MaxGuests,
    string PropertyType
);
