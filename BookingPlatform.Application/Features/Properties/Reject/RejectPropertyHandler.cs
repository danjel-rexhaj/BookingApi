using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Properties.Reject
{
    public class RejectPropertyHandler
        : IRequestHandler<RejectPropertyCommand, Unit>
    {
        private readonly IPropertyRepository _repository;
        private readonly INotificationRepository _notificationRepository;
        public RejectPropertyHandler(IPropertyRepository repository, 
            INotificationRepository notificationRepository)
        {
            _repository = repository;
            _notificationRepository = notificationRepository;
        }

        public async Task<Unit> Handle(
            RejectPropertyCommand request,
            CancellationToken cancellationToken)
        {
            var property = await _repository.GetByIdAsync(request.PropertyId);

            if (property == null)
                throw new Exception("Property not found");

            property.Reject();


            var notification = new Notification(
                property.OwnerId,
                "Your property has been rejected.",
                "PropertyRejected");

            await _notificationRepository.AddAsync(notification);
            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}