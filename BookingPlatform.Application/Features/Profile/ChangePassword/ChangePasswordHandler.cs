using BCrypt.Net;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Profile.ChangePassword;

public class ChangePasswordHandler
    : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notificationRepository;

    public ChangePasswordHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUser,
        INotificationRepository notificationRepository)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_currentUser.UserId);

        if (user == null)
            throw new Exception("User not found");

        // verifiko password aktual
        var validPassword = BCrypt.Net.BCrypt.Verify(
            request.CurrentPassword,
            user.PasswordHash);

        if (!validPassword)
            throw new Exception("Current password is incorrect");

        // hash password i ri
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        user.ChangePassword(hashedPassword);

        // notification
        var notification = new Notification(
            user.Id,
            "Your password has been changed successfully.",
            "PasswordChanged");

        await _notificationRepository.AddAsync(notification);

        await _userRepository.SaveChangesAsync();

        return Unit.Value;
    }
}