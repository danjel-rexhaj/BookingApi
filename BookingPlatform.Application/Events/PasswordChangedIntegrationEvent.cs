using System;

namespace BookingPlatform.Application.Events;

public class PasswordChangedIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public DateTime ChangedAtUtc { get; init; }
}