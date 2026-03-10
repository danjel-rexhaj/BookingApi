using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Features.Properties.Approve
{
    public class ApprovePropertyHandler : IRequestHandler<ApprovePropertyCommand>
    {
        private readonly IPropertyRepository _repository;
        private readonly INotificationRepository _notificationRepository;
        public ApprovePropertyHandler(IPropertyRepository repository, INotificationRepository notificationRepository)
        {
            _repository = repository;
            _notificationRepository = notificationRepository;
        }

        public async Task<Unit> Handle(ApprovePropertyCommand request, CancellationToken cancellationToken)
        {
            var property = await _repository.GetByIdAsync(request.PropertyId);

            if (property == null)
                throw new Exception("Property not found");

            property.Approve();   

            var notification = new Notification(
                property.OwnerId,
                "Your property has been approved.",
                "PropertyApproved");

            await _notificationRepository.AddAsync(notification);
            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
