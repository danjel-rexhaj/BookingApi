using BookingPlatform.Application.Events;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using Hangfire;
using MediatR;
using System.Text.Json;

namespace BookingPlatform.Application.Features.Properties.Approve;

public class ApprovePropertyHandler : IRequestHandler<ApprovePropertyCommand, Unit>
{
    private readonly IPropertyRepository _repository;
    private readonly IPropertyImageRepository _propertyImageRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IOutboxRepository _outboxRepository;

    public ApprovePropertyHandler(
        IPropertyRepository repository,
        IPropertyImageRepository propertyImageRepository,
        INotificationRepository notificationRepository,
        INotificationService notificationService,
        IUserRepository userRepository,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        IOutboxRepository outboxRepository)
    {
        _repository = repository;
        _propertyImageRepository = propertyImageRepository;
        _notificationRepository = notificationRepository;
        _notificationService = notificationService;
        _userRepository = userRepository;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _outboxRepository = outboxRepository;
    }

    public async Task<Unit> Handle(
        ApprovePropertyCommand request,
        CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.PropertyId);
        if (property == null)
            throw new Exception("Property not found.");

        var images = await _propertyImageRepository.GetByPropertyIdAsync(property.Id);
        if (images.Count < 3)
            throw new Exception("A property must have at least 3 images before approval.");

        var owner = await _userRepository.GetByIdAsync(property.OwnerId);
        if (owner == null)
            throw new Exception("Owner not found.");

        var approvedAtUtc = DateTime.UtcNow;

        property.Approve();

        var message = "Your property has been approved.";

        var notification = new Notification(
            property.OwnerId,
            message,
            "PropertyApproved"
        );

        await _notificationRepository.AddAsync(notification);
        await _repository.SaveChangesAsync();

        var propertyApprovedEvent = new PropertyApprovedIntegrationEvent
        {
            PropertyId = property.Id,
            OwnerId = property.OwnerId,
            PropertyName = property.Name,
            City = property.Address.City,
            Country = property.Address.Country,
            PropertyType = property.PropertyType,
            MaxGuests = property.MaxGuests,
            PricePerNight = property.BasePricePerNight,
            ApprovedAtUtc = approvedAtUtc
        };

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(PropertyApprovedIntegrationEvent),
            Payload = JsonSerializer.Serialize(propertyApprovedEvent),
            OccurredOnUtc = DateTime.UtcNow
        };

        await _outboxRepository.AddAsync(outboxMessage);
        await _outboxRepository.SaveChangesAsync();

        await _notificationService.SendNotificationAsync(
            property.OwnerId,
            message
        );

        var emailBody = _emailTemplateService.GetPropertyApprovedEmail(
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
                "Your property has been approved",
                emailBody
            )
        );

        return Unit.Value;
    }
}