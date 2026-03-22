using BookingPlatform.Domain.Entities;
using System.Net.NetworkInformation;

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

    public ICollection<Booking> Bookings { get; private set; } = new List<Booking>();





    public int? MinimumStay { get; private set; }
    public int? MaximumStay { get; private set; }
    /*
    public decimal? DiscountPercentage { get; private set; }
    public DateTime? DiscountValidFrom { get; private set; }
    public DateTime? DiscountValidTo { get; private set; }
    */
    public decimal BasePricePerNight { get; set; }

    public ICollection<PropertyAmenity> Amenities { get; private set; } = new List<PropertyAmenity>();// lidhja me amenity permes nje table many-to-many

    public Property(Guid ownerId, Guid addressId, string name,
                    string description, string propertyType, int maxGuests,
                    TimeSpan checkInTime, TimeSpan checkOutTime, decimal basePricePerNight)
    {
        Id = Guid.NewGuid();
        OwnerId = ownerId;
        AddressId = addressId;
        Name = name;
        Description = description;
        PropertyType = propertyType;
        MaxGuests = maxGuests;
        CheckInTime = checkInTime;
        CheckOutTime = checkOutTime;
        BasePricePerNight = basePricePerNight;

        IsActive = false;
        IsApproved = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void Approve()
    {
        IsApproved = true;
        IsActive = true;
        LastModifiedAt = DateTime.UtcNow;
    }


    public void Suspend()
    {
        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        IsApproved = false;
        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
    }


    public void SetStayLimits(int? minimumStay, int? maximumStay)
    {
        MinimumStay = minimumStay;
        MaximumStay = maximumStay;
    }


    public void UpdateDetails(
        string name,
        string description,
        string propertyType,
        int maxGuests,
        decimal basePricePerNight,
        TimeSpan checkInTime,
        TimeSpan checkOutTime)
    {
        Name = name;
        Description = description;
        PropertyType = propertyType;
        MaxGuests = maxGuests;
        BasePricePerNight = basePricePerNight;
        CheckInTime = checkInTime;
        CheckOutTime = checkOutTime;

        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdateAmenities(List<Guid> amenityIds)
    {
        Amenities.Clear();

        foreach (var amenityId in amenityIds)
        {
            Amenities.Add(new PropertyAmenity(Id, amenityId));
        }

        LastModifiedAt = DateTime.UtcNow;
    }

}