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

public class SeasonalPriceRepository : ISeasonalPriceRepository
{
    private readonly BookingDbContext _context;

    public SeasonalPriceRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<List<SeasonalPrice>>
        GetByPropertyIdAsync(Guid propertyId)
    {
        return await _context.SeasonalPrices
            .Where(s => s.PropertyId == propertyId)
            .ToListAsync();
    }

    public async Task AddAsync(SeasonalPrice seasonalPrice)
    {
        await _context.SeasonalPrices.AddAsync(seasonalPrice);
        await _context.SaveChangesAsync();
    }
}
