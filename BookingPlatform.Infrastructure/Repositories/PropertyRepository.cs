using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly BookingDbContext _context;

    public PropertyRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Property property)
    {
        await _context.Properties.AddAsync(property);
    }

    public async Task<Property?> GetByIdAsync(Guid id)
    {
        return await _context.Properties
            .Include(p => p.Address)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    // dependency injection of BookingDbContext and other repositories if needed
    //service lifetime is typically scoped for repositories in web applications, meaning a new instance is created per request. This allows for efficient resource management and ensures that the same context is used throughout the request lifecycle, which is important for maintaining data consistency and managing transactions effectively.

    public async Task<List<Property>> SearchAsync(
        string? country,
        string? city,
        int? guests,
        string? propertyType,
        string? amenities,
        string? rating,
        string? price,
        DateTime? startDate,
        DateTime? endDate,
        string? sortBy)
        {
    var query = _context.Properties
        .Include(p => p.Address)
        .Include(p => p.Bookings)
        .Include(p => p.Amenities)
        .Where(p => p.IsApproved && p.IsActive)
        .AsQueryable();

    
        if (!string.IsNullOrWhiteSpace(country))
            query = query.Where(p => p.Address.Country == country);


        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(p => p.Address.City == city);


        if (guests.HasValue)
            query = query.Where(p => p.MaxGuests >= guests.Value);


        if (!string.IsNullOrWhiteSpace(propertyType))
            query = query.Where(p => p.PropertyType == propertyType);


        if (!string.IsNullOrWhiteSpace(price))
        {
            var priceValue = decimal.Parse(price);
            query = query.Where(p => p.BasePricePerNight <= priceValue);
        }


        if (!string.IsNullOrWhiteSpace(rating))
        {
            var ratingValue = double.Parse(rating);

            query = query.Where(p =>
                _context.Reviews
                .Where(r => r.Booking.PropertyId == p.Id)
                .Average(r => (double?)r.Rating) >= ratingValue);
        }


        if (!string.IsNullOrWhiteSpace(amenities))
        {
            query = query.Where(p =>
                p.Amenities.Any(a => a.Amenity.Name == amenities));
        }


        if (startDate.HasValue && endDate.HasValue)
        {
            var start = startDate.Value;
            var end = endDate.Value;

            query = query.Where(p =>
                !p.Bookings.Any(b =>
                    b.BookingStatus == Domain.Enums.BookingStatus.Confirmed &&
                    b.StartDate < end &&
                    b.EndDate > start));
        }


        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            switch (sortBy.ToLower())
            {
                case "price":
                    query = query.OrderBy(p => p.BasePricePerNight);
                    break;

                case "rating":
                    query = query.OrderByDescending(p =>
                        _context.Reviews
                            .Where(r => r.Booking.PropertyId == p.Id)
                            .Average(r => (double?)r.Rating) ?? 0);
                    break;

                case "popularity":
                    query = query.OrderByDescending(p => p.Bookings.Count);
                    break;

                default:
                    query = query.OrderBy(p => p.Name);
                    break;
            }
        }

        return await query.ToListAsync();
    }


    public async Task<double> GetAverageRatingAsync(Guid propertyId)
    {
        var reviews = await _context.Reviews
            .Include(r => r.Booking)
            .Where(r => r.Booking.PropertyId == propertyId)
            .ToListAsync();

        if (!reviews.Any())
            return 0;

        return reviews.Average(r => r.Rating);
    }


}