using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Profile.UpdateProfile;

public class UpdateProfileHandler
    : IRequestHandler<UpdateProfileCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notificationRepository;

    public UpdateProfileHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUser,
        INotificationRepository notificationRepository)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        _notificationRepository = notificationRepository;
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

        var notification = new Notification(
            user.Id,
            "Your profile information has been updated.",
            "ProfileUpdated");

        await _notificationRepository.AddAsync(notification);

        await _userRepository.SaveChangesAsync();

        return Unit.Value;
    }
}