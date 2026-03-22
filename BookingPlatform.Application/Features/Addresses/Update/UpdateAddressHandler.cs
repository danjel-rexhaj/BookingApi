using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Addresses.Update
{
    public class UpdateAddressHandler
        : IRequestHandler<UpdateAddressCommand, Unit>
    {
        private readonly IAddressRepository _repository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUser;

        public UpdateAddressHandler(
            IAddressRepository repository,
            INotificationRepository notificationRepository,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _notificationRepository = notificationRepository;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(
            UpdateAddressCommand request,
            CancellationToken cancellationToken)
        {
            var address = await _repository.GetByIdAsync(request.Id);

            if (address == null)
                throw new Exception("Address not found");

            address.Update(
                request.Country,
                request.City,
                request.Street,
                request.PostalCode
            );

            var notification = new Notification(
                _currentUser.UserId,
                $"Address in {request.City}, {request.Country} has been updated.",
                "AddressUpdated"
            );

            await _notificationRepository.AddAsync(notification);

            await _repository.SaveChangesAsync();

            return Unit.Value;  //nuk kthen asgje sepse eshte unit htjshte behet ndryshimi 
        }
    }
}