using System;

namespace BookingPlatform.Application.Events;

public class OwnerProfileCreatedIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string BusinessName { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
}