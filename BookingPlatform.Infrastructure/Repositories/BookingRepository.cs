using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace BookingPlatform.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext _context;

    public BookingRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
            return null;

        var now = DateTime.UtcNow;

        booking.ExpireIfNeeded(now);
        booking.CompleteIfFinished(now);

        await _context.SaveChangesAsync();

        return booking;
    }

    public async Task<List<Booking>> GetBookingsForPropertyAsync(Guid propertyId)
    {
        return await _context.Bookings
            .Where(b => b.PropertyId == propertyId)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Booking>> GetBookingsForUserAsync(Guid userId)
    {
        var bookings = await _context.Bookings
            .Where(b => b.GuestId == userId)
            .ToListAsync();

        var now = DateTime.UtcNow;

        foreach (var booking in bookings)
        {
            booking.ExpireIfNeeded(now);
            booking.CompleteIfFinished(now);
        }

        await _context.SaveChangesAsync();

        return bookings;
    }

    public async Task<List<SeasonalPrice>> GetSeasonalPricesForPropertyAsync(Guid propertyId)
    {
        return await _context.SeasonalPrices
            .Where(sp => sp.PropertyId == propertyId)
            .ToListAsync();
    }


    public async Task<List<Booking>> GetAllAsync()
    {
        return await _context.Bookings.ToListAsync();
    }
}