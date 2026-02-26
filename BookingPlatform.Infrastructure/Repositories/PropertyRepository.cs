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
    string? propertyType)
    {
        var query = _context.Properties
            .Where(p => p.IsApproved && p.IsActive)
            .Include(p => p.Address)
            .AsQueryable();

        if (!string.IsNullOrEmpty(city))
            query = query.Where(p => p.Address.City == city);

        if (guests.HasValue)
            query = query.Where(p => p.MaxGuests >= guests.Value);

        if (!string.IsNullOrEmpty(propertyType))
            query = query.Where(p => p.PropertyType == propertyType);

        return await query.ToListAsync();
    }
}