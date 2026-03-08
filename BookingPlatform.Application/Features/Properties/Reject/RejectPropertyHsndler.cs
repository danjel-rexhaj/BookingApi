using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Properties.Reject
{
    public class RejectPropertyHandler
        : IRequestHandler<RejectPropertyCommand, Unit>
    {
        private readonly IPropertyRepository _repository;

        public RejectPropertyHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(
            RejectPropertyCommand request,
            CancellationToken cancellationToken)
        {
            var property = await _repository.GetByIdAsync(request.PropertyId);

            if (property == null)
                throw new Exception("Property not found");

            property.Reject();

            await _repository.SaveChangesAsync();


            var notification = new Notification(
                property.OwnerId,
                "Your property has been rejected.",
                "PropertyRejected");

            return Unit.Value;
        }
    }
}