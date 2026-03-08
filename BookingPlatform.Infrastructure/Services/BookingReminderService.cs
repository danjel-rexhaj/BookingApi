using BookingPlatform.Infrastructure.Persistence;
using BookingPlatform.Domain.Enums;
using BookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class BookingReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public BookingReminderService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

            var tomorrow = DateTime.UtcNow.Date.AddDays(1);

            var bookings = await context.Bookings
                .Where(b => b.BookingStatus == BookingStatus.Confirmed &&
                            b.StartDate.Date == tomorrow)
                .ToListAsync();

            foreach (var booking in bookings)
            {
                var notification = new Notification(
                    booking.GuestId,
                    "Booking Reminder",
                    $"Reminder: Your stay starts tomorrow."
                );

                context.Notifications.Add(notification);
            }

            await context.SaveChangesAsync();

            await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
        }
    }
}