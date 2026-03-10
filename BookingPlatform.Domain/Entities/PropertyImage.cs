using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Domain.Entities;

public class PropertyImage
{
    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public string ImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }


    public PropertyImage(Guid propertyId, string imageUrl)
    {
        Id = Guid.NewGuid();
        PropertyId = propertyId;
        ImageUrl = imageUrl;
        CreatedAt = DateTime.UtcNow;
    }
}
