namespace BookingPlatform.Domain.Entities;

public class Property
{
    public Guid Id { get; private set; }

    public Guid OwnerId { get; private set; }
    public User Owner { get; private set; } = null!;

    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string PropertyType { get; private set; } = null!;

    public Guid AddressId { get; private set; }
    public Address Address { get; private set; } = null!;

    public int MaxGuests { get; private set; }
    public TimeSpan CheckInTime { get; private set; }
    public TimeSpan CheckOutTime { get; private set; }

    public bool IsActive { get; private set; }
    public bool IsApproved { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public DateTime? LastBookedOnUtc { get; private set; }

    private Property() { }

    public Property(Guid ownerId, Guid addressId, string name, string description, string propertyType, int maxGuests)
    {
        Id = Guid.NewGuid();
        OwnerId = ownerId;
        AddressId = addressId;
        Name = name;
        Description = description;
        PropertyType = propertyType;
        MaxGuests = maxGuests;
        IsActive = true;
        IsApproved = false;
        CreatedAt = DateTime.UtcNow;
    }
}
