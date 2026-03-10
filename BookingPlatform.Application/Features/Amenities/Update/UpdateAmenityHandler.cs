using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Amenities.Update;

public class UpdateAmenityHandler
    : IRequestHandler<UpdateAmenityCommand, Unit>
{
    private readonly IAmenityRepository _repository;
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUser;

    public UpdateAmenityHandler(
        IAmenityRepository repository,
        INotificationRepository notificationRepository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _notificationRepository = notificationRepository;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        UpdateAmenityCommand request,
        CancellationToken cancellationToken)
    {
        var amenity = await _repository.GetByIdAsync(request.Id);

        if (amenity == null)
            throw new Exception("Amenity not found");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new Exception("Amenity name cannot be empty");

        amenity.UpdateName(request.Name);

        var notification = new Notification(
            _currentUser.UserId,
            $"Amenity '{request.Name}' has been updated.",
            "AmenityUpdated"
        );

        await _notificationRepository.AddAsync(notification);

        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}