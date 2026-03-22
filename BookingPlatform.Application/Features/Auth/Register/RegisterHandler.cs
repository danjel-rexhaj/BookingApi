using BookingPlatform.Application.Events;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using Hangfire;
using MediatR;
using System.Text.Json;

namespace BookingPlatform.Application.Features.Auth.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, string>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRoleRepository _roleRepository;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IOutboxRepository _outboxRepository;

    public RegisterHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        IOutboxRepository outboxRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _outboxRepository = outboxRepository;
    }

    public async Task<string> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser != null)
            throw new Exception("User already exists");

        var hashedPassword = _passwordHasher.Hash(request.Password);
        var registeredAtUtc = DateTime.UtcNow;

        var user = new User(
            request.FirstName,
            request.LastName,
            request.Email,
            hashedPassword
        );

        var guestRole = await _roleRepository.GetByNameAsync("Guest");

        if (guestRole == null)
            throw new Exception("Guest role not found");

        user.UserRoles.Add(new UserRole(
            user.Id,
            guestRole.Id,
            registeredAtUtc
        ));

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var userRegisteredEvent = new UserRegisteredIntegrationEvent
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            RoleName = guestRole.Name,
            RegisteredAtUtc = registeredAtUtc
        };

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(UserRegisteredIntegrationEvent),
            Payload = JsonSerializer.Serialize(userRegisteredEvent),
            OccurredOnUtc = DateTime.UtcNow
        };

        await _outboxRepository.AddAsync(outboxMessage);
        await _outboxRepository.SaveChangesAsync();

        var emailBody = _emailTemplateService.GetWelcomeEmail(user.FirstName);

        BackgroundJob.Enqueue(() =>
            _emailService.SendEmailAsync(
                user.Email,
                "Welcome to Booking Platform",
                emailBody
            )
        );

        return "User registered successfully";
    }
}