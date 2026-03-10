using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Amenities.Create;

public class CreateAmenityHandler
    : IRequestHandler<CreateAmenityCommand, Guid>
{
    private readonly IAmenityRepository _repository;
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUser;

    public CreateAmenityHandler(
        IAmenityRepository repository,
        INotificationRepository notificationRepository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _notificationRepository = notificationRepository;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(
        CreateAmenityCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new Exception("Amenity name is required");

        var amenity = new Amenity(request.Name);

        await _repository.AddAsync(amenity);

        var notification = new Notification(
            _currentUser.UserId,
            $"Amenity '{request.Name}' has been created.",
            "AmenityCreated"
        );

        await _notificationRepository.AddAsync(notification);

        await _repository.SaveChangesAsync();

        return amenity.Id;
    }
}