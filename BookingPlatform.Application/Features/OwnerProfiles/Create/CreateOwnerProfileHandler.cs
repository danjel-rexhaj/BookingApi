using BookingPlatform.Application.Events;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using Hangfire;
using MediatR;
using System.Text.Json;

namespace BookingPlatform.Application.Features.OwnerProfiles.Create;

public class CreateOwnerProfileHandler : IRequestHandler<CreateOwnerProfileCommand, Unit>
{
    private readonly IOwnerProfileRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IOutboxRepository _outboxRepository;

    public CreateOwnerProfileHandler(
        IOwnerProfileRepository repository,
        ICurrentUserService currentUser,
        INotificationRepository notificationRepository,
        INotificationService notificationService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        IOutboxRepository outboxRepository)
    {
        _repository = repository;
        _currentUser = currentUser;
        _notificationRepository = notificationRepository;
        _notificationService = notificationService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _outboxRepository = outboxRepository;
    }

    public async Task<Unit> Handle(
        CreateOwnerProfileCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found.");

        var existingProfile = await _repository.GetByUserIdAsync(userId);
        if (existingProfile != null)
            throw new Exception("Owner profile already exists.");

        var createdAtUtc = DateTime.UtcNow;

        var profile = new OwnerProfile(
            userId,
            request.IdentityCardNumber,
            request.BusinessName,
            request.CreditCard
        );

        await _repository.AddAsync(profile);

        var ownerRole = await _roleRepository.GetByNameAsync("Owner");
        if (ownerRole == null)
            throw new Exception("Owner role not found.");

        var isAdmin = user.UserRoles.Any(ur => ur.Role.Name == "Admin");
        var hasOwnerRole = user.UserRoles.Any(ur => ur.RoleId == ownerRole.Id);

        if (!isAdmin && !hasOwnerRole)
        {
            user.AddRole(ownerRole.Id);
        }

        var message = "Your owner profile has been created successfully.";

        var notification = new Notification(
            userId,
            message,
            "OwnerProfileCreated"
        );

        await _notificationRepository.AddAsync(notification);
        await _repository.SaveChangesAsync();

        var ownerProfileCreatedEvent = new OwnerProfileCreatedIntegrationEvent
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            Email = user.Email,
            BusinessName = request.BusinessName,
            CreatedAtUtc = createdAtUtc
        };

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(OwnerProfileCreatedIntegrationEvent),
            Payload = JsonSerializer.Serialize(ownerProfileCreatedEvent),
            OccurredOnUtc = DateTime.UtcNow
        };

        await _outboxRepository.AddAsync(outboxMessage);
        await _outboxRepository.SaveChangesAsync();

        await _notificationService.SendNotificationAsync(
            userId,
            message
        );

        var emailBody = _emailTemplateService.GetOwnerProfileCreatedEmail(
            user.FirstName,
            request.BusinessName
        );

        BackgroundJob.Enqueue(() =>
            _emailService.SendEmailAsync(
                user.Email,
                "Welcome as an owner",
                emailBody
            )
        );

        return Unit.Value;
    }
}