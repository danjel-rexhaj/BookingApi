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
    using BookingPlatform.Domain.Entities;
    using MediatR;

    public class UpdatePropertyHandler
        : IRequestHandler<UpdatePropertyCommand, Unit>
    {
        private readonly IPropertyRepository _repository;
        private readonly IPropertyRuleRepository _propertyRuleRepository;
        private readonly INotificationRepository _notificationRepository;
        public UpdatePropertyHandler(IPropertyRepository repository, 
            IPropertyRuleRepository propertyAmenityRepository, 
            INotificationRepository notificationRepository)
        {
            _repository = repository;
            _propertyRuleRepository = propertyAmenityRepository;
            _notificationRepository = notificationRepository;
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
            // update rules
            await _propertyRuleRepository.DeleteByPropertyIdAsync(property.Id);

            foreach (var rule in request.Rules)
            {
                var propertyRule = new PropertyRule(property.Id, rule);
                await _propertyRuleRepository.AddAsync(propertyRule);
            }
            await _repository.SaveChangesAsync();

            var notification = new Notification(
                property.OwnerId,
                "Your property has been updated.",
                "PropertyUpdated");

            await _notificationRepository.AddAsync(notification);
            await _repository.SaveChangesAsync();


            return Unit.Value;
        }
    }
}
