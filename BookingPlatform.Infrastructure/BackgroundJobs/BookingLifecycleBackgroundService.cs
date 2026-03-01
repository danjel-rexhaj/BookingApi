using BookingPlatform.Infrastructure.Persistence;
using BookingPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookingPlatform.Infrastructure.BackgroundJobs;

public class BookingLifecycleBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public BookingLifecycleBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

            var now = DateTime.UtcNow;

            // Fshij Pending Bookings (24h)
            var expiredBookings = await context.Bookings
                .Where(b => b.BookingStatus == BookingStatus.Pending &&
                            b.CreatedOnUtc.AddHours(24) < now)
                .ToListAsync(stoppingToken);

            foreach (var booking in expiredBookings)
            {
                booking.MarkAsExpired();
            }

            // Konfirmo Bookings
            var completedBookings = await context.Bookings
                .Where(b => b.BookingStatus == BookingStatus.Confirmed &&
                            b.EndDate < now)
                .ToListAsync(stoppingToken);

            foreach (var booking in completedBookings)
            {
                booking.MarkAsCompleted();
            }

            await context.SaveChangesAsync(stoppingToken);

            // Vonese 5 minuta
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}