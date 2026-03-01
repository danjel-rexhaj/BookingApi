using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user, IEnumerable<string> roles);
    string GenerateRefreshToken(Guid userId);
    Guid? ValidateRefreshToken(string refreshToken);
    void RemoveRefreshToken(string refreshToken);
}
