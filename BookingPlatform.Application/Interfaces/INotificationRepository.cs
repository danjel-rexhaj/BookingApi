using BookingPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<List<Notification>> GetUserNotificationsAsync(Guid userId);
        Task<Notification?> GetByIdAsync(Guid id);
        Task SaveChangesAsync();
    }
}
