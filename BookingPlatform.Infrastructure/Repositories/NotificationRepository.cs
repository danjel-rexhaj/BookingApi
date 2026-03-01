using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace BookingPlatform.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly BookingDbContext _context;

        public NotificationRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
