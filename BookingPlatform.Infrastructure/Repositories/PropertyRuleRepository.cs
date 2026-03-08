using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Infrastructure.Persistence;

public class PropertyRuleRepository : IPropertyRuleRepository
{
    private readonly BookingDbContext _context;

    public PropertyRuleRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PropertyRule rule)
    {
        await _context.Set<PropertyRule>().AddAsync(rule);
        await _context.SaveChangesAsync();
    }
}