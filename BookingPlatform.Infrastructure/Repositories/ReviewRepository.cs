using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly BookingDbContext _context;

    public ReviewRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsForBookingAsync(Guid bookingId)
    {
        return await _context.Reviews
            .AnyAsync(r => r.BookingId == bookingId);
    }

    public async Task<List<Review>> GetReviewsForPropertyAsync(Guid propertyId)
    {
        return await _context.Reviews
            .Include(r => r.Booking)
            .Where(r => r.Booking.PropertyId == propertyId)
            .ToListAsync();
    }
}