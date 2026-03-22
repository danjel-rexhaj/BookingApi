using BookingPlatform.Domain.Entities;
using MediatR;
using System;
using BookingPlatform.Application.Interfaces;


namespace BookingPlatform.Application.Features.Properties.Update;

public class UpdatePropertyAmenitiesHandler
    : IRequestHandler<UpdatePropertyAmenitiesCommand, Unit>
{
    private readonly IPropertyRepository _repository;
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationService _notificationService;
    public UpdatePropertyAmenitiesHandler(IPropertyRepository repository, 
        INotificationRepository notificationRepository,
        INotificationService notificationService)

    {
        _repository = repository;
        _notificationRepository = notificationRepository;
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(UpdatePropertyAmenitiesCommand request, CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.PropertyId);

        if (property == null)
            throw new Exception("Property not found");

        property.UpdateAmenities(request.AmenityIds);

        var message = "Your amenities has been updated.";

        var notification = new Notification(
            property.OwnerId,
            "Your amenities has been updated.",
            "AmenitiesUpdated");

        await _notificationRepository.AddAsync(notification);
        await _repository.SaveChangesAsync();

        await _notificationService.SendNotificationAsync(
           property.OwnerId,
           message
       );

        return Unit.Value;
    }
}