using BookingPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Interfaces
{
    public interface IPropertyRuleRepository
    {
        Task<List<PropertyRule>> GetByPropertyIdAsync(Guid propertyId);

        Task AddAsync(PropertyRule rule);

        Task DeleteByPropertyIdAsync(Guid propertyId);
    }
}
