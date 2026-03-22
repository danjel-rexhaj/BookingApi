using System;

namespace BookingPlatform.Application.Events;

public class BookingCanceledIntegrationEvent : IntegrationEvent
{
    public Guid BookingId { get; init; }
    public Guid GuestId { get; init; }
    public Guid PropertyId { get; init; }
    public DateTime CheckInDate { get; init; }
    public DateTime CheckOutDate { get; init; }
    public DateTime CanceledAtUtc { get; init; }
}