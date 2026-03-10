using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PropertyRuleRepository : IPropertyRuleRepository
{
    private readonly BookingDbContext _context;

    public PropertyRuleRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PropertyRule rule)
    {
        await _context.PropertyRules.AddAsync(rule);
    }

    public async Task DeleteByPropertyIdAsync(Guid propertyId)
    {
        var rules = await _context.PropertyRules
            .Where(r => r.PropertyId == propertyId)
            .ToListAsync();

        _context.PropertyRules.RemoveRange(rules);
    }

    public async Task<List<PropertyRule>> GetByPropertyIdAsync(Guid propertyId)
    {
        return await _context.PropertyRules
            .Where(r => r.PropertyId == propertyId)
            .ToListAsync();
    }
}