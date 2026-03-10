using BookingPlatform.Infrastructure.Persistence;
using BookingPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.BackgroundJobs;

public class BookingLifecycleJob
{
    private readonly BookingDbContext _context;

    public BookingLifecycleJob(BookingDbContext context)
    {
        _context = context;
    }

    public async Task ProcessBookings()
    {
        var now = DateTime.UtcNow;

        // Expire Pending bookings pas 24h
        var expiredBookings = await _context.Bookings
            .Where(b => b.BookingStatus == BookingStatus.Pending &&
                        b.CreatedOnUtc.AddHours(24) < now)
            .ToListAsync();

        foreach (var booking in expiredBookings)
        {
            booking.MarkAsExpired();
        }

        // Complete bookings qe kane perfunduar
        var completedBookings = await _context.Bookings
            .Where(b => b.BookingStatus == BookingStatus.Confirmed &&
                        b.EndDate < now)
            .ToListAsync();

        foreach (var booking in completedBookings)
        {
            booking.MarkAsCompleted();
        }

        await _context.SaveChangesAsync();
    }
}