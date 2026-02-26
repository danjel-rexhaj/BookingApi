using BookingPlatform.Application.Properties.Create;
using BookingPlatform.Domain.Entities;
using MediatR;
using BookingPlatform.Application.Interfaces;

namespace BookingPlatform.Application.Properties.Create;

public class CreatePropertyHandler
    : IRequestHandler<CreatePropertyCommand, Guid>
{
    private readonly IPropertyRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CreatePropertyHandler(
        IPropertyRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(
        CreatePropertyCommand request,
        CancellationToken cancellationToken)
    {
        var property = new Property(
            _currentUser.UserId,
            request.AddressId,
            request.Name,
            request.Description,
            request.PropertyType,
            request.MaxGuests,
            request.CheckInTime,
            request.CheckOutTime
        );

        await _repository.AddAsync(property);

        return property.Id;
    }
}
