using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Interfaces;

public interface IPropertyRepository
{
    Task AddAsync(Property property);
    Task<Property?> GetByIdAsync(Guid id);
    Task SaveChangesAsync();

    Task<List<Property>> SearchAsync(
        string? country,
        string? city,
        int? guests,
        string? propertyType,
        string? amenities,
        string? rating,
        string? price,
        DateTime? startDate,
        DateTime? endDate,
        string? sortBy);
}

