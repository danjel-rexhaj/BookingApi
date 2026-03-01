using BookingPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Interfaces;

public interface IReviewRepository
{
    Task AddAsync(Review review);
    Task<bool> ExistsForBookingAsync(Guid bookingId);
    Task<List<Review>> GetReviewsForPropertyAsync(Guid propertyId);
}
