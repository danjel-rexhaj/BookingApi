using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using MediatR;

namespace BookingPlatform.Application.Features.Profile.UpdateProfile;

public class UpdateProfileHandler
    : IRequestHandler<UpdateProfileCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;

    public UpdateProfileHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        UpdateProfileCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository
            .GetByIdAsync(_currentUser.UserId);

        if (user == null)
            throw new Exception("User not found");

        user.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.PhoneNumber);

        await _userRepository.SaveChangesAsync();

        return Unit.Value;
    }
}