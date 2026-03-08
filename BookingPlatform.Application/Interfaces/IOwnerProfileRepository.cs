using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Interfaces
{
    public interface IOwnerProfileRepository
    {
        Task AddAsync(OwnerProfile profile);
        Task<OwnerProfile?> GetByUserIdAsync(Guid userId);
        Task SaveChangesAsync();
    }
}