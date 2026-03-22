using System;

namespace BookingPlatform.Application.Events;

public class BookingCreatedIntegrationEvent : IntegrationEvent
{
    public Guid BookingId { get; init; }
    public Guid GuestId { get; init; }
    public Guid PropertyId { get; init; }
    public DateTime CheckInDate { get; init; }
    public DateTime CheckOutDate { get; init; }
    public decimal TotalPrice { get; init; }
}