using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Properties.BlockDate;

public class AddBlockedDateHandler
    : IRequestHandler<AddBlockedDateCommand, Guid>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IBlockedDateRepository _blockedDateRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notificationRepository;

    public AddBlockedDateHandler(
        IPropertyRepository propertyRepository,
        IBlockedDateRepository blockedDateRepository,
        ICurrentUserService currentUser,
        INotificationRepository notificationRepository)
    {
        _propertyRepository = propertyRepository;
        _blockedDateRepository = blockedDateRepository;
        _currentUser = currentUser;
        _notificationRepository = notificationRepository;
    }

    public async Task<Guid> Handle(
        AddBlockedDateCommand request,
        CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);

        if (property == null)
            throw new Exception("Property not found");

        if (property.OwnerId != _currentUser.UserId)
            throw new Exception("Unauthorized");

        var blockedDate = new BlockedDate(
            request.PropertyId,
            request.Date
        );

        await _blockedDateRepository.AddAsync(blockedDate);

        var message = $"Date {request.Date:yyyy-MM-dd} has been blocked for your property.";

        var notification = new Notification(
            property.OwnerId,
            message,
            "BlockedDateAdded"
        );

        await _notificationRepository.AddAsync(notification);

        await _propertyRepository.SaveChangesAsync();

        return blockedDate.Id;
    }
}