using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using MediatR;
using BCrypt.Net;

namespace BookingPlatform.Application.Features.Profile.ChangePassword;

public class ChangePasswordHandler
    : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;

    public ChangePasswordHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository
            .GetByIdAsync(_currentUser.UserId);

        if (user == null)
            throw new Exception("User not found");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        user.ChangePassword(hashedPassword);

        await _userRepository.SaveChangesAsync();

        return Unit.Value;
    }
}
