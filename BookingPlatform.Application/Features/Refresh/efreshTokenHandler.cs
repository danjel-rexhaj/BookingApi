using BookingPlatform.Application.Features.Auth;
using BookingPlatform.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Refresh
{
    public class RefreshTokenHandler
    : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public RefreshTokenHandler(
            IUserRepository userRepository,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            var userId = _jwtService.ValidateRefreshToken(request.RefreshToken);

            if (userId == null)
                throw new Exception("Invalid refresh token");

            var user = await _userRepository.GetByIdAsync(userId.Value);

            if (user == null)
                throw new Exception("User not found");

            var roles = user.UserRoles
                .Where(r => r.Role != null)
                .Select(r => r.Role!.Name);

            var newAccessToken = _jwtService.GenerateToken(user, roles);
            var newRefreshToken = _jwtService.GenerateRefreshToken(user.Id);

            _jwtService.RemoveRefreshToken(request.RefreshToken);

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}
