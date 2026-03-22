using System;

namespace BookingPlatform.Application.Events;

public class UserRegisteredIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string RoleName { get; init; } = string.Empty;
    public DateTime RegisteredAtUtc { get; init; }
}