using BookingPlatform.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Properties.Update
{
    using BookingPlatform.Application.Interfaces;
    using MediatR;

    public class UpdatePropertyHandler
        : IRequestHandler<UpdatePropertyCommand, Unit>
    {
        private readonly IPropertyRepository _repository;
        private readonly IPropertyRuleRepository _propertyRuleRepository;
        public UpdatePropertyHandler(IPropertyRepository repository, IPropertyRuleRepository propertyAmenityRepository)
        {
            _repository = repository;
            _propertyRuleRepository = propertyAmenityRepository;
        }

        public async Task<Unit> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
        {
            var property = await _repository.GetByIdAsync(request.PropertyId);

            if (property == null)
                throw new Exception("Property not found");

            property.UpdateDetails(
                request.Name,
                request.Description,
                request.PropertyType,
                request.MaxGuests,
                request.BasePricePerNight,
                request.CheckInTime,
                request.CheckOutTime
            );

            property.SetStayLimits(
                request.MinimumStay,
                request.MaximumStay
            );

            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
