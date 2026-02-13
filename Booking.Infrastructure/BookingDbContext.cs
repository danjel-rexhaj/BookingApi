using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Microsoft.EntityFrameworkCore;
{
    
}

namespace Booking.Infrastructure
{
    internal class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> op) : base(op)
        {
            DbSet<User> Users { get; set; }

        }
    }
}
