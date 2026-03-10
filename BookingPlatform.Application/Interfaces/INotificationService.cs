using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookingPlatform.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(Guid userId, string message);
}
