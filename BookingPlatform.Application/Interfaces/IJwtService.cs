using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user, IEnumerable<string> roles);
}
