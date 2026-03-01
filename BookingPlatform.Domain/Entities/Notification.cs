using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }

        public string Message { get; private set; } = null!;
        public string Type { get; private set; } = null!;

        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Notification() { }

        public Notification(Guid userId, string message, string type)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Message = message;
            Type = type;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
