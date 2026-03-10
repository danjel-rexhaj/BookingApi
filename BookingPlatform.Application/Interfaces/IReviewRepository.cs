using BookingPlatform.Domain.Entities;


namespace BookingPlatform.Application.Interfaces;

public interface IReviewRepository
{
    Task AddAsync(Review review);
    Task<bool> ExistsForBookingAsync(Guid bookingId);
    Task<List<Review>> GetReviewsForPropertyAsync(Guid propertyId);
}
