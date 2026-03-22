using System;

namespace BookingPlatform.Application.Events;

public class ReviewCreatedIntegrationEvent : IntegrationEvent
{
    public Guid ReviewId { get; init; }
    public Guid BookingId { get; init; }
    public Guid GuestId { get; init; }
    public Guid PropertyId { get; init; }
    public Guid OwnerId { get; init; }
    public int Rating { get; init; }
    public string Comment { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
}