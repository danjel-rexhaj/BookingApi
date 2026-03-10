using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Admin.Users.DeleteUser;

public class DeleteUserHandler
    : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;

    public DeleteUserHandler(
        IUserRepository userRepository,
        INotificationRepository notificationRepository)
    {
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
            throw new Exception("User not found");

        var notification = new Notification(
            user.Id,
            "Your account has been deleted by the administrator.",
            "UserDeleted"
        );

        await _notificationRepository.AddAsync(notification);

        await _userRepository.DeleteAsync(user);
        await _userRepository.SaveChangesAsync();

        return Unit.Value;
    }
}