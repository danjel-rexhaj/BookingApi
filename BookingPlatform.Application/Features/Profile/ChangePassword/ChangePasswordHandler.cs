using BookingPlatform.Application.Events;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using Hangfire;
using MediatR;
using System.Text.Json;

namespace BookingPlatform.Application.Features.Profile.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationService _notificationService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IOutboxRepository _outboxRepository;

    public ChangePasswordHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUser,
        INotificationRepository notificationRepository,
        INotificationService notificationService,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        IOutboxRepository outboxRepository)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        _notificationRepository = notificationRepository;
        _notificationService = notificationService;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _outboxRepository = outboxRepository;
    }

    public async Task<Unit> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_currentUser.UserId);

        if (user == null)
            throw new Exception("User not found");

        var validPassword = _passwordHasher.Verify(
            user.PasswordHash,
            request.CurrentPassword);

        if (!validPassword)
            throw new Exception("Current password is incorrect");

        var hashedPassword = _passwordHasher.Hash(request.NewPassword);
        var changedAtUtc = DateTime.UtcNow;

        user.ChangePassword(hashedPassword);

        var message = "Your password has been changed successfully.";

        var notification = new Notification(
            user.Id,
            message,
            "PasswordChanged"
        );

        await _notificationRepository.AddAsync(notification);
        await _userRepository.SaveChangesAsync();

        var passwordChangedEvent = new PasswordChangedIntegrationEvent
        {
            UserId = user.Id,
            Email = user.Email,
            ChangedAtUtc = changedAtUtc
        };

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(PasswordChangedIntegrationEvent),
            Payload = JsonSerializer.Serialize(passwordChangedEvent),
            OccurredOnUtc = DateTime.UtcNow
        };

        await _outboxRepository.AddAsync(outboxMessage);
        await _outboxRepository.SaveChangesAsync();

        await _notificationService.SendNotificationAsync(
            user.Id,
            message
        );

        var emailBody = _emailTemplateService.GetPasswordChangedEmail(
            user.FirstName
        );

        BackgroundJob.Enqueue(() =>
            _emailService.SendEmailAsync(
                user.Email,
                "Password changed successfully",
                emailBody
            )
        );

        return Unit.Value;
    }
}