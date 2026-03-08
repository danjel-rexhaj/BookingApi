using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Persistence.Repositories
{
    public class OwnerProfileRepository : IOwnerProfileRepository
    {
        private readonly BookingDbContext _context;

        public OwnerProfileRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OwnerProfile profile)
        {
            await _context.OwnerProfiles.AddAsync(profile);
        }

        public async Task<OwnerProfile?> GetByUserIdAsync(Guid userId)
        {
            return await _context.OwnerProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}