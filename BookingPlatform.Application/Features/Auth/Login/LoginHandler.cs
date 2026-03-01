using BookingPlatform.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Features.Auth.Login;

public class LoginHandler
    : IRequestHandler<LoginQuery, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(
        LoginQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
            throw new Exception("Invalid credentials");

        var isValid = _passwordHasher.Verify(user.PasswordHash, request.Password);

        if (!isValid)
            throw new Exception("Invalid credentials");

        var roles = user.UserRoles.Select(ur => ur.Role.Name);

        var accessToken = _jwtService.GenerateToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken(user.Id);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}