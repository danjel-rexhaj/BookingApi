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
        await _context.SaveChangesAsync();
    }

    public async Task<Property?> GetByIdAsync(Guid id)
    {
        return await _context.Properties
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Property>> SearchAsync(
        string? city,
        int? guests,
        string? propertyType,
        DateTime? startDate,
        DateTime? endDate,
        string? sortBy,
        int page,
        int pageSize)
    {
        var query = _context.Properties
            .Include(p => p.Address)
            .Include(p => p.Bookings)
            .Where(p => p.IsApproved && p.IsActive)
            .AsQueryable();

 
        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(p => p.Address.City == city);


        if (guests.HasValue)
            query = query.Where(p => p.MaxGuests >= guests.Value);

       
        if (!string.IsNullOrWhiteSpace(propertyType))
            query = query.Where(p => p.PropertyType == propertyType);

       
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

            query = query.OrderBy(p => p.Name);
        }

        
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        query = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

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