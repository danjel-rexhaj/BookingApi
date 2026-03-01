using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using MediatR;

namespace BookingPlatform.Application.Features.Profile.UpdateProfileImage;

public class UpdateProfileImageHandler
    : IRequestHandler<UpdateProfileImageCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;

    public UpdateProfileImageHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        UpdateProfileImageCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository
            .GetByIdAsync(_currentUser.UserId);

        if (user == null)
            throw new Exception("User not found");

        user.UpdateProfileImage(request.ImageUrl);

        await _userRepository.SaveChangesAsync();

        return Unit.Value;
    }
}
