using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Interfaces
{
    public interface IAmenityRepository
    {
        Task AddAsync(Amenity amenity);
        Task<List<Amenity>> GetAllAsync();

        Task<Amenity?> GetByIdAsync(Guid id);
        Task SaveChangesAsync();

    }


}