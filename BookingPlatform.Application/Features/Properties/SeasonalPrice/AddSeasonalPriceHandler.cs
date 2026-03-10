using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;


namespace BookingPlatform.Application.Features.Properties.SeasonalPrice;

public class AddSeasonalPriceHandler
    : IRequestHandler<AddSeasonalPriceCommand, Guid>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ISeasonalPriceRepository _seasonalRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notificationRepository;

    public AddSeasonalPriceHandler(
        IPropertyRepository propertyRepository,
        ISeasonalPriceRepository seasonalRepository,
        ICurrentUserService currentUser,
        INotificationRepository notificationRepository)
    {
        _propertyRepository = propertyRepository;
        _seasonalRepository = seasonalRepository;
        _currentUser = currentUser;
        _notificationRepository = notificationRepository;
    }

    public async Task<Guid> Handle(
        AddSeasonalPriceCommand request,
        CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);

        if (property == null)
            throw new Exception("Property not found");

        if (property.OwnerId != _currentUser.UserId)
            throw new Exception("Unauthorized");

        if (request.StartDate >= request.EndDate)
            throw new Exception("Invalid date range");


        var seasonalPrice = new Domain.Entities.SeasonalPrice(
                   request.PropertyId,
                   request.StartDate,
                   request.EndDate,
                   request.PricePerNight
               );

        await _seasonalRepository.AddAsync(seasonalPrice);

        var message = $"Seasonal price added from {request.StartDate:yyyy-MM-dd} " +
                      $"to {request.EndDate:yyyy-MM-dd} " +
                      $"with price {request.PricePerNight}€/night.";

        var notification = new Notification(
            property.OwnerId,
            message,
            "SeasonalPriceAdded"
        );

        await _notificationRepository.AddAsync(notification);

        await _propertyRepository.SaveChangesAsync();

        return seasonalPrice.Id;
    }
}