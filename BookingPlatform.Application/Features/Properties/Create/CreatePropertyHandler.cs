using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Properties.Create;

public class CreatePropertyHandler : IRequestHandler<CreatePropertyCommand, Guid>
{
    private readonly IPropertyRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IPropertyRuleRepository _propertyRuleRepository;
    private readonly IPropertyAmenityRepository _propertyAmenityRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;

    public CreatePropertyHandler(
        IPropertyRepository repository,
        ICurrentUserService currentUser,
        IPropertyRuleRepository propertyRuleRepository,
        IPropertyAmenityRepository propertyAmenityRepository,
        INotificationRepository notificationRepository,
        INotificationService notificationService,
        IUserRepository userRepository)
    {
        _repository = repository;
        _currentUser = currentUser;
        _propertyRuleRepository = propertyRuleRepository;
        _propertyAmenityRepository = propertyAmenityRepository;
        _notificationRepository = notificationRepository;
        _notificationService = notificationService;
        _userRepository = userRepository;
    }

    public async Task<Guid> Handle(
        CreatePropertyCommand request,
        CancellationToken cancellationToken)
    {
        var ownerId = _currentUser.UserId;

        var user = await _userRepository.GetByIdAsync(ownerId);
        if (user == null)
            throw new Exception("User not found.");

        var isOwner = user.UserRoles.Any(ur => ur.Role.Name == "Owner");
        if (!isOwner)
            throw new Exception("Only owners can create properties.");

        var property = new Property(
            ownerId,
            request.AddressId,
            request.Name,
            request.Description,
            request.PropertyType,
            request.MaxGuests,
            request.CheckInTime,
            request.CheckOutTime,
            request.BasePricePerNight
        );

        property.SetStayLimits(request.MinimumStay, request.MaximumStay);

        await _repository.AddAsync(property);

        foreach (var ruleText in request.Rules)
        {
            var rule = new PropertyRule(property.Id, ruleText);
            await _propertyRuleRepository.AddAsync(rule);
        }

        foreach (var amenityId in request.AmenityIds)
        {
            var propertyAmenity = new PropertyAmenity(property.Id, amenityId);
            await _propertyAmenityRepository.AddAsync(propertyAmenity);
        }

        var message = "Your property has been created and is pending admin approval.";

        var notification = new Notification(
            property.OwnerId,
            message,
            "PropertyCreated"
        );

        await _notificationRepository.AddAsync(notification);
        await _repository.SaveChangesAsync();

        await _notificationService.SendNotificationAsync(
            property.OwnerId,
            message
        );

        return property.Id;
    }
}