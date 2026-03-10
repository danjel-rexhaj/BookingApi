using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Properties.Suspend;

public class SuspendPropertyHandler
    : IRequestHandler<SuspendPropertyCommand>
{
    private readonly IPropertyRepository _repository;
    private readonly INotificationRepository _notificationRepository;
    public SuspendPropertyHandler(IPropertyRepository repository, 
        INotificationRepository notificationRepository)
    {
        _repository = repository;
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(
        SuspendPropertyCommand request,
        CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.PropertyId);

        if (property == null)
            throw new Exception("Property not found");

        property.Suspend();


        var notification = new Notification(
            property.OwnerId,
            "Your property has been suspended.",
            "PropertySuspended");

        await _notificationRepository.AddAsync(notification);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}
