using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Infrastructure.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace BookingPlatform.Infrastructure.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hub;

    public SignalRNotificationService(IHubContext<NotificationHub> hub)
    {
        _hub = hub;
    }

    public async Task SendNotificationAsync(Guid userId, string message)
    {
        await _hub.Clients.User(userId.ToString())
            .SendAsync("ReceiveNotification", message);
    }
}