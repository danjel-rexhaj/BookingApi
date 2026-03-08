using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Interfaces
{
    public interface IPropertyImageRepository
    {
        Task AddAsync(PropertyImage image);

        Task<List<PropertyImage>> GetByPropertyIdAsync(Guid propertyId);

        Task SaveChangesAsync();
    }
}