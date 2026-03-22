namespace BookingPlatform.Domain.Entities;

public class Address
{
    public Guid Id { get; private set; }

    public string Country { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string Street { get; private set; } = null!;
    public string PostalCode { get; private set; } = null!;

    public ICollection<Property> Properties { get; private set; } = new List<Property>(); // Navigation property for related properties, nje adrese permban shume prona 

    private Address() { }

    public Address(string country, string city, string street, string postalCode)
    {
        Id = Guid.NewGuid();
        Country = country;
        City = city;
        Street = street;
        PostalCode = postalCode;
    }


    public void Update(string country, string city, string street, string postalCode)
    {
        Country = country;
        City = city;
        Street = street;
        PostalCode = postalCode;
    }
}
