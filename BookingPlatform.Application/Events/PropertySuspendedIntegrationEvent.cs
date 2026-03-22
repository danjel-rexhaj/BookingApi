using System;

namespace BookingPlatform.Application.Events;

public class PropertySuspendedIntegrationEvent : IntegrationEvent
{
    public Guid PropertyId { get; init; }
    public Guid OwnerId { get; init; }
    public string PropertyName { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string PropertyType { get; init; } = string.Empty;
    public int MaxGuests { get; init; }
    public decimal PricePerNight { get; init; }
    public DateTime SuspendedAtUtc { get; init; }
}