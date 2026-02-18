namespace BookingPlatform.Domain.Entities;

public class OwnerProfile
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string IdentityCardNumber { get; private set; } = null!;
    public string VerificationStatus { get; private set; } = null!;
    public string? BusinessName { get; private set; }
    public string? CreditCard { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }

    private OwnerProfile() { }

    public OwnerProfile(Guid userId, string identityCardNumber)
    {
        UserId = userId;
        IdentityCardNumber = identityCardNumber;
        VerificationStatus = "Pending";
        CreatedAt = DateTime.UtcNow;
    }
}
