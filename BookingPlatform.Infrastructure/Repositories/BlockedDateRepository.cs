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

public class BlockedDateRepository : IBlockedDateRepository
{
    private readonly BookingDbContext _context;

    public BlockedDateRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<List<BlockedDate>>
        GetByPropertyIdAsync(Guid propertyId)
    {
        return await _context.BlockedDates
            .Where(b => b.PropertyId == propertyId)
            .ToListAsync();
    }


    public async Task AddAsync(BlockedDate blockedDate)
    {
        await _context.BlockedDates.AddAsync(blockedDate);
        await _context.SaveChangesAsync();
    }
}
