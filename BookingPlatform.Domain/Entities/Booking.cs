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


    public void SetPricing(
    decimal cleaningFee,
    decimal amenitiesUpCharge,
    decimal priceForPeriod,
    decimal totalPrice)
    {
        CleaningFee = cleaningFee;
        AmenitiesUpCharge = amenitiesUpCharge;
        PriceForPeriod = priceForPeriod;
        TotalPrice = totalPrice;
    }



    public void Confirm(DateTime utcNow)
    {
        if (BookingStatus != BookingStatus.Pending)
            throw new Exception("Only pending bookings can be confirmed.");

        BookingStatus = BookingStatus.Confirmed;
        ConfirmedOnUtc = utcNow;
        LastModifiedAt = utcNow;
    }

    public void Reject(DateTime utcNow)
    {
        if (BookingStatus != BookingStatus.Pending)
            throw new Exception("Only pending bookings can be rejected.");

        BookingStatus = BookingStatus.Rejected;
        RejectedOnUtc = utcNow;
        LastModifiedAt = utcNow;
    }

    public void Cancel(DateTime utcNow)
    {
        // zakonisht lejojme cancel kur eshte Pending ose Confirmed (para se te filloje)
        if (BookingStatus != BookingStatus.Pending && BookingStatus != BookingStatus.Confirmed)
            throw new Exception("Only pending or confirmed bookings can be cancelled.");

        BookingStatus = BookingStatus.Cancelled;
        CancelledOnUtc = utcNow;
        LastModifiedAt = utcNow;
    }

}
