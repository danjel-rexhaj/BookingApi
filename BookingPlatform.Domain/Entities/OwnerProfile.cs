using System;

namespace BookingPlatform.Domain.Entities
{
    public class OwnerProfile
    {
        public Guid UserId { get; private set; }

        public string IdentityCardNumber { get; private set; }
       
        public string BusinessName { get; private set; }
        public string CreditCard { get; private set; }
        public User User { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }

        private OwnerProfile() { } // EF Core

        public OwnerProfile(
            Guid userId,
            string identityCardNumber,
            string businessName,
            string creditCard)
        {
            UserId = userId;
            IdentityCardNumber = identityCardNumber;
            BusinessName = businessName;
            CreditCard = creditCard;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
        string identityCardNumber,
        string businessName,
        string creditCard)
        {
            IdentityCardNumber = identityCardNumber;
            BusinessName = businessName;
            CreditCard = creditCard;

            LastModifiedAt = DateTime.UtcNow;
        }
    }
}