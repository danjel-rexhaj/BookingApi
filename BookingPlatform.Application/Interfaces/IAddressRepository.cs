using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Interfaces
{
    public interface IAddressRepository
    {
        Task AddAsync(Address address);
        Task<List<Address>> GetAllAsync();

        Task<Address?> GetByIdAsync(Guid id);
        Task SaveChangesAsync();
    }
}