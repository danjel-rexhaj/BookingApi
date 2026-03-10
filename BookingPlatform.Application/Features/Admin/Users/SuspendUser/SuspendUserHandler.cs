using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Admin.Users.SuspendUser;

public class SuspendUserHandler
    : IRequestHandler<SuspendUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;

    public SuspendUserHandler(
        IUserRepository userRepository,
        INotificationRepository notificationRepository)
    {
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(
        SuspendUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
            throw new Exception("User not found");

        user.Deactivate();

        var notification = new Notification(
            user.Id,
            "Your account has been suspended by the administrator.",
            "UserSuspended"
        );

        await _notificationRepository.AddAsync(notification);

        await _userRepository.SaveChangesAsync();

        return Unit.Value;
    }
}