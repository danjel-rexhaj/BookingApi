using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Profile.UpdateProfileImage;

public class UpdateProfileImageHandler
    : IRequestHandler<UpdateProfileImageCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notificationRepository;

    public UpdateProfileImageHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUser,
        INotificationRepository notificationRepository)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        _notificationRepository = notificationRepository;
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

        var notification = new Notification(
            user.Id,
            "Your profile image has been updated.",
            "ProfileImageUpdated");

        await _notificationRepository.AddAsync(notification);

        await _userRepository.SaveChangesAsync();

        return Unit.Value;
    }
}