using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Domain.Entities;

public class PropertyAmenity
{
    public Guid PropertyId { get; private set; }
    public Property Property { get; private set; } = null!;

    public Guid AmenityId { get; private set; }
    public Amenity Amenity { get; private set; } = null!;

    private PropertyAmenity() { }

    public PropertyAmenity(Guid propertyId, Guid amenityId)
    {
        PropertyId = propertyId;
        AmenityId = amenityId;
    }
}