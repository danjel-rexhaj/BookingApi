using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Interfaces;

public interface IBookingRepository
{
    Task AddAsync(Booking booking);
    Task<Booking?> GetByIdAsync(Guid id);
    Task<List<Booking>> GetBookingsForPropertyAsync(Guid propertyId);
    Task<List<Booking>> GetBookingsForUserAsync(Guid userId);
    Task<List<SeasonalPrice>> GetSeasonalPricesForPropertyAsync(Guid propertyId);
    Task<List<Booking>> GetAllAsync();
    Task SaveChangesAsync();
}
