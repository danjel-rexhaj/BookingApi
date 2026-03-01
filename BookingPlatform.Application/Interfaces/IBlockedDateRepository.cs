using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Interfaces;

public interface IBlockedDateRepository
{
    Task<List<BlockedDate>> GetByPropertyIdAsync(Guid propertyId);
    Task AddAsync(BlockedDate blockedDate);

}
