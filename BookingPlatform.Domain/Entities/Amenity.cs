using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Domain.Entities;

public class Amenity
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    private Amenity() { }

    public Amenity(string name)
    {
        Id = Guid.NewGuid(); // id unike f3c2e5c7-7c4c-4f3d-b2d1-5c5c9b2f2d3a
        Name = name;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }
}
