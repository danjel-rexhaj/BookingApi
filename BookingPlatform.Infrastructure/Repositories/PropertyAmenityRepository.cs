using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Persistence.Repositories
{
    public class PropertyAmenityRepository : IPropertyAmenityRepository
    {
        private readonly BookingDbContext _context;

        public PropertyAmenityRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PropertyAmenity propertyAmenity)
        {
            await _context.PropertyAmenities.AddAsync(propertyAmenity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}