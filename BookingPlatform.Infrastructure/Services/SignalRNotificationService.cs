using BookingPlatform.Application.Interfaces;
using BookingPlatform.Infrastructure.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace BookingPlatform.Infrastructure.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(Guid userId, string message)
    {
        await _hubContext.Clients
            .Group(userId.ToString())
            .SendAsync("ReceiveNotification", message);
    }
}


/*  await _hub.Clients.All
     .SendAsync("ReceiveNotification", message);
*/
//testimin e ben nga.user ne ALL nga swagger hap file 
//file:///C:/Users/User/Desktop/signalr-testt.html