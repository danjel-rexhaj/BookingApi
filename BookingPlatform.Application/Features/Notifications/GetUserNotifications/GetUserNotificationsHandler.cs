using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Notifications.GetUserNotifications;

public class GetUserNotificationsHandler
    : IRequestHandler<GetUserNotificationsQuery, List<Notification>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUser;

    public GetUserNotificationsHandler(
        INotificationRepository notificationRepository,
        ICurrentUserService currentUser)
    {
        _notificationRepository = notificationRepository;
        _currentUser = currentUser;
    }

    public async Task<List<Notification>> Handle(
        GetUserNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        return await _notificationRepository.GetUserNotificationsAsync(userId);
    }
}