using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Persistence.Repositories
{
    public class PropertyImageRepository : IPropertyImageRepository
    {
        private readonly BookingDbContext _context;

        public PropertyImageRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PropertyImage image)
        {
            await _context.PropertyImages.AddAsync(image);
        }

        public async Task<List<PropertyImage>> GetByPropertyIdAsync(Guid propertyId)
        {
            return await _context.PropertyImages
                .Where(x => x.PropertyId == propertyId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}