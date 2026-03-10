using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Admin.Users.ChangeRoles;

public class ChangeUserRoleHandler : IRequestHandler<ChangeUserRoleCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly INotificationRepository _notificationRepository;

    public ChangeUserRoleHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        INotificationRepository notificationRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
            throw new Exception("User not found");

        var role = await _roleRepository.GetByNameAsync(request.RoleName);

        if (role == null)
            throw new Exception("Role not found");

        user.UserRoles.Clear();

        user.UserRoles.Add(
            new UserRole(user.Id, role.Id, DateTime.UtcNow)
        );

        var notification = new Notification(
            user.Id,
            $"Your role has been changed to {request.RoleName}.",
            "UserRoleChanged"
        );

        await _notificationRepository.AddAsync(notification);

        await _userRepository.SaveChangesAsync();

        return Unit.Value;
    }
}