using BookingPlatform.Application.Events;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using Hangfire;
using MediatR;
using System.Text.Json;

namespace BookingPlatform.Application.Features.Properties.Suspend;

public class SuspendPropertyHandler : IRequestHandler<SuspendPropertyCommand, Unit>
{
    private readonly IPropertyRepository _repository;
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IOutboxRepository _outboxRepository;

    public SuspendPropertyHandler(
        IPropertyRepository repository,
        INotificationRepository notificationRepository,
        INotificationService notificationService,
        IUserRepository userRepository,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        IOutboxRepository outboxRepository)
    {
        _repository = repository;
        _notificationRepository = notificationRepository;
        _notificationService = notificationService;
        _userRepository = userRepository;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _outboxRepository = outboxRepository;
    }

    public async Task<Unit> Handle(
        SuspendPropertyCommand request,
        CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.PropertyId);
        if (property == null)
            throw new Exception("Property not found.");

        var owner = await _userRepository.GetByIdAsync(property.OwnerId);
        if (owner == null)
            throw new Exception("Owner not found.");

        var suspendedAtUtc = DateTime.UtcNow;

        property.Suspend();

        var message = "Your property has been suspended.";

        var notification = new Notification(
            property.OwnerId,
            message,
            "PropertySuspended"
        );

        await _notificationRepository.AddAsync(notification);
        await _repository.SaveChangesAsync();

        var propertySuspendedEvent = new PropertySuspendedIntegrationEvent
        {
            PropertyId = property.Id,
            OwnerId = property.OwnerId,
            PropertyName = property.Name,
            City = property.Address.City,
            Country = property.Address.Country,
            PropertyType = property.PropertyType,
            MaxGuests = property.MaxGuests,
            PricePerNight = property.BasePricePerNight,
            SuspendedAtUtc = suspendedAtUtc
        };

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(PropertySuspendedIntegrationEvent),
            Payload = JsonSerializer.Serialize(propertySuspendedEvent),
            OccurredOnUtc = DateTime.UtcNow
        };

        await _outboxRepository.AddAsync(outboxMessage);
        await _outboxRepository.SaveChangesAsync();

        await _notificationService.SendNotificationAsync(
            property.OwnerId,
            message
        );

        var emailBody = _emailTemplateService.GetPropertySuspendedEmail(
            owner.FirstName,
            property.Name,
            property.Address.City,
            property.Address.Country,
            property.PropertyType,
            property.MaxGuests,
            property.BasePricePerNight
        );

        BackgroundJob.Enqueue(() =>
            _emailService.SendEmailAsync(
                owner.Email,
                "Your property has been suspended",
                emailBody
            )
        );

        return Unit.Value;
    }
}