namespace BookingPlatform.Domain.Entities;

public class Review
{
    public Guid Id { get; private set; }

    public Guid BookingId { get; private set; }
    public Booking Booking { get; private set; } = null!;

    public Guid GuestId { get; private set; }
    public User Guest { get; private set; } = null!;

    public int Rating { get; private set; }
    public string Comment { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    private Review() { }

    public Review(Guid bookingId, Guid guestId, int rating, string comment)
    {
        Id = Guid.NewGuid();
        BookingId = bookingId;
        GuestId = guestId;
        Rating = rating;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
    }
}
