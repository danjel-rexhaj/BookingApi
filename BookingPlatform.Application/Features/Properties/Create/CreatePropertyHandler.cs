using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Properties.Create;

public class CreatePropertyHandler
    : IRequestHandler<CreatePropertyCommand, Guid>
{
    private readonly IPropertyRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IPropertyRuleRepository _propertyRuleRepository;
    public CreatePropertyHandler(
        IPropertyRepository repository,
        ICurrentUserService currentUser,
        IPropertyRuleRepository propertyAmenityRepository)
    {
        _repository = repository;
        _currentUser = currentUser;
        _propertyRuleRepository = propertyAmenityRepository;
    }

    public async Task<Guid> Handle(
    CreatePropertyCommand request,
    CancellationToken cancellationToken)
    {
        var ownerId = _currentUser.UserId;

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

        var notification = new Notification(
            property.OwnerId,
            "Your property has been created.",
            "PropertyCreated");

        return property.Id;
    }



}
