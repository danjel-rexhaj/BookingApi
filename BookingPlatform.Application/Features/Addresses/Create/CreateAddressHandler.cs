using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Addresses.Create
{
    public class CreateAddressHandler
        : IRequestHandler<CreateAddressCommand, Guid>
    {
        private readonly IAddressRepository _repository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUser;

        public CreateAddressHandler(
            IAddressRepository repository,
            INotificationRepository notificationRepository,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _notificationRepository = notificationRepository;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(
            CreateAddressCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Country))
                throw new Exception("Country is required");

            if (string.IsNullOrWhiteSpace(request.City))
                throw new Exception("City is required");

            if (string.IsNullOrWhiteSpace(request.Street))
                throw new Exception("Street is required");

            var address = new Address(
                request.Country,
                request.City,
                request.Street,
                request.PostalCode
            );

            await _repository.AddAsync(address);

            var notification = new Notification(
                _currentUser.UserId,
                $"Address in {request.City}, {request.Country} has been created.",
                "AddressCreated"
            );

            await _notificationRepository.AddAsync(notification);

            await _repository.SaveChangesAsync();

            return address.Id;
        }
    }
}