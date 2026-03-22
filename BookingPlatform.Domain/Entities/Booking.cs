using BookingPlatform.Domain.Enums; //sepse booking status eshte enum

namespace BookingPlatform.Domain.Entities;

public class Booking
{
    public Guid Id { get; private set; }

    public Guid PropertyId { get; private set; } //foreign key e property
    public Property Property { get; private set; } = null!; //objekti i plote

    public Guid GuestId { get; private set; }
    public User Guest { get; private set; } = null!;

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public int GuestCount { get; private set; }

    public decimal CleaningFee { get; private set; }
    public decimal AmenitiesUpCharge { get; private set; }
    public decimal PriceForPeriod { get; private set; }
    public decimal TotalPrice { get; private set; }
    public decimal? RefundAmount { get; private set; }
    public BookingStatus BookingStatus { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ConfirmedOnUtc { get; private set; }
    public DateTime? RejectedOnUtc { get; private set; }
    public DateTime? CompletedOnUtc { get; private set; }
    public DateTime? CancelledOnUtc { get; private set; }

    public decimal TaxAmount { get; private set; }
    private Booking() { }

    public Booking(Guid propertyId, Guid guestId, DateTime startDate, DateTime endDate, int guestCount)  // konstruktori per te krijuar nje booking te ri, pa i vendosur cmimet dhe statusin
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


    public void SetPricing( //metode per te vendosur cmimet pasi te llogariten
        decimal cleaningFee,
        decimal amenitiesUpCharge,
        decimal priceForPeriod,
        decimal taxAmount,
        decimal totalPrice)
    {
        CleaningFee = cleaningFee;
        AmenitiesUpCharge = amenitiesUpCharge;
        PriceForPeriod = priceForPeriod;
        TaxAmount = taxAmount;
        TotalPrice = totalPrice;
    }



    public void Confirm(DateTime utcNow) //metode per te konfirmuar booking, ndryshon statusin dhe vendos daten e konfirmimit
    {
        if (BookingStatus != BookingStatus.Pending)
            throw new Exception("Only pending bookings can be confirmed.");

        BookingStatus = BookingStatus.Confirmed;
        ConfirmedOnUtc = utcNow;
        LastModifiedAt = utcNow; //cordinated universal time utcNOW
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
       
        if (BookingStatus != BookingStatus.Pending && BookingStatus != BookingStatus.Confirmed)
            throw new Exception("Only pending or confirmed bookings can be cancelled.");

        BookingStatus = BookingStatus.Cancelled;
        CancelledOnUtc = utcNow;
        LastModifiedAt = utcNow;
    }




    public void MarkAsExpired()
    {
        if (BookingStatus != BookingStatus.Pending)
            throw new InvalidOperationException("Only pending bookings can expire.");

        BookingStatus = BookingStatus.Expired;
    }



    public void MarkAsCompleted()
    {
        if (BookingStatus != BookingStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed bookings can be completed.");

        BookingStatus = BookingStatus.Completed;
        CompletedOnUtc = DateTime.UtcNow;
    }


    public void CancelWithPolicy()
    {
        if (BookingStatus != BookingStatus.Pending &&
            BookingStatus != BookingStatus.Confirmed)
            throw new InvalidOperationException("Cannot cancel this booking.");

        var now = DateTime.UtcNow;
        var hoursBeforeStart = (StartDate - now).TotalHours;

        decimal refundPercentage;

        if (hoursBeforeStart > 48)
            refundPercentage = 1m;//100% refund if cancelled more than 48 hours before start
        else if (hoursBeforeStart > 24)
            refundPercentage = 0.5m; //50% refund if cancelled between 24 and 48 hours before start
        else
            refundPercentage = 0m; //no refund if cancelled less than 24 hours before start

        RefundAmount = TotalPrice * refundPercentage;

        BookingStatus = BookingStatus.Cancelled;
        CancelledOnUtc = now;
    }

    public void ExpireIfNeeded(DateTime utcNow)
    {
        if (BookingStatus == BookingStatus.Pending &&
            CreatedOnUtc.AddHours(24) < utcNow)
        {
            BookingStatus = BookingStatus.Expired;
        }
    }

    public void CompleteIfFinished(DateTime utcNow)
    {
        if (BookingStatus == BookingStatus.Confirmed &&
            EndDate < utcNow)
        {
            BookingStatus = BookingStatus.Completed;
            CompletedOnUtc = utcNow;
        }
    }


}
