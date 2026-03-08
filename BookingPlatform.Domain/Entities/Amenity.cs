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
        Id = Guid.NewGuid();
        Name = name;
    }
}
