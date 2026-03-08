using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Persistence.Repositories
{
    public class AmenityRepository : IAmenityRepository
    {
        private readonly BookingDbContext _context;

        public AmenityRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Amenity amenity)
        {
            await _context.Amenities.AddAsync(amenity);
        }

        public async Task<List<Amenity>> GetAllAsync()
        {
            return await _context.Amenities.ToListAsync();
        }

        public async Task<Amenity?> GetByIdAsync(Guid id)
        {
            return await _context.Amenities
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}