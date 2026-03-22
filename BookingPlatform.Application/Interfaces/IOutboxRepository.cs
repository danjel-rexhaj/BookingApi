using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Interfaces;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message);
    Task SaveChangesAsync();
}