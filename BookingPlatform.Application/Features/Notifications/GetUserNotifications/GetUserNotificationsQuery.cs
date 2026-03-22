using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Features.Notifications.GetUserNotifications;

public class GetUserNotificationsQuery : IRequest<List<Notification>>
{
}