using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Interfaces
{
    public interface IPropertyAmenityRepository
    {
        Task AddAsync(PropertyAmenity propertyAmenity);

        Task SaveChangesAsync();
    }
}