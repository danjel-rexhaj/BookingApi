using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.OwnerProfiles.Create;

public class CreateOwnerProfileHandler
    : IRequestHandler<CreateOwnerProfileCommand, Unit>
{
    private readonly IOwnerProfileRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notificationRepository;

    public CreateOwnerProfileHandler(
        IOwnerProfileRepository repository,
        ICurrentUserService currentUser,
        INotificationRepository notificationRepository)
    {
        _repository = repository;
        _currentUser = currentUser;
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(
        CreateOwnerProfileCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var profile = new OwnerProfile(
            userId,
            request.IdentityCardNumber,
            request.BusinessName,
            request.CreditCard
        );

        await _repository.AddAsync(profile);

        var notification = new Notification(
            userId,
            "Your owner profile has been created successfully.",
            "OwnerProfileCreated"
        );

        await _notificationRepository.AddAsync(notification);

        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}