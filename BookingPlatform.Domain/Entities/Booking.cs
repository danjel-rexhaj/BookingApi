using BookingPlatform.Domain.Enums;

namespace BookingPlatform.Domain.Entities;

public class Booking
{
    public Guid Id { get; private set; }

    public Guid PropertyId { get; private set; }
    public Property Property { get; private set; } = null!;

    public Guid GuestId { get; private set; }
    public User Guest { get; private set; } = null!;

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public int GuestCount { get; private set; }

    public decimal CleaningFee { get; private set; }
    public decimal AmenitiesUpCharge { get; private set; }
    public decimal PriceForPeriod { get; private set; }
    public decimal TotalPrice { get; private set; }

    public BookingStatus BookingStatus { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ConfirmedOnUtc { get; private set; }
    public DateTime? RejectedOnUtc { get; private set; }
    public DateTime? CompletedOnUtc { get; private set; }
    public DateTime? CancelledOnUtc { get; private set; }

    private Booking() { }

    public Booking(Guid propertyId, Guid guestId, DateTime startDate, DateTime endDate, int guestCount)
    {
        Id = Guid.NewGuid();
        PropertyId = propertyId;
        GuestId = guestId;
        StartDate = startDate;
        EndDate = endDate;
        GuestCount = guestCount;

        BookingStatus = BookingStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        CreatedOnUtc = DateTime.UtcNow;
    }
}
