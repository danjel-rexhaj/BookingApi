using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.OwnerProfiles.Update;

public class UpdateOwnerProfileHandler
    : IRequestHandler<UpdateOwnerProfileCommand, Unit>
{
    private readonly IOwnerProfileRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationService _notificationService;

    public UpdateOwnerProfileHandler(
        IOwnerProfileRepository repository,
        ICurrentUserService currentUser,
        INotificationRepository notificationRepository,
        INotificationService notificationService)
    {
        _repository = repository;
        _currentUser = currentUser;
        _notificationRepository = notificationRepository;
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(
        UpdateOwnerProfileCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var profile = await _repository.GetByUserIdAsync(userId);

        if (profile == null)
            throw new Exception("Owner profile not found");

        profile.Update(
            request.IdentityCardNumber,
            request.BusinessName,
            request.CreditCard
        );

        var message = "Your owner profile has been updated.";

        var notification = new Notification(
            userId,
            message,
            "OwnerProfileUpdated"
        );

        await _notificationRepository.AddAsync(notification);
        await _repository.SaveChangesAsync();

        await _notificationService.SendNotificationAsync(
            userId,
            message
        );

        return Unit.Value;
    }
}