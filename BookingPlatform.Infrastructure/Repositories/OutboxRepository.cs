using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Infrastructure.Persistence;

namespace BookingPlatform.Infrastructure.Repositories;

public class OutboxRepository : IOutboxRepository
{
    private readonly BookingDbContext _context;

    public OutboxRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OutboxMessage message)
    {
        await _context.OutboxMessages.AddAsync(message);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}