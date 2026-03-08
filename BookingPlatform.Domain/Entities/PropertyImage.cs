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
    public Property Property { get; private set; } = null!;

    public string ImageUrl { get; private set; } = null!;

    private PropertyImage() { }

    public PropertyImage(Guid propertyId, string imageUrl)
    {
        Id = Guid.NewGuid();
        PropertyId = propertyId;
        ImageUrl = imageUrl;
    }
}
