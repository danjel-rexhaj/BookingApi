using MediatR;

namespace BookingPlatform.Application.Features.Properties.Create;

public record CreatePropertyCommand(
    Guid AddressId,
    string Name,
    string Description,
    string PropertyType,
    int MaxGuests,
    TimeSpan CheckInTime,
    TimeSpan CheckOutTime,
    decimal BasePricePerNight,
    int? MinimumStay,
    int? MaximumStay,
    List<string> Rules,
    List<Guid> AmenityIds
) : IRequest<Guid>;