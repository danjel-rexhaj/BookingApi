using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Interfaces;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
    Task<User?> GetByEmailAsync(string email);
    Task<User> GetByIdAsync(Guid id);
    Task<List<User>> GetAllAsync();
    Task DeleteAsync(User user);

}
