using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookingPlatform.Domain.Entities;

public class BlockedDate
{
    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public DateTime Date { get; private set; }

    private BlockedDate() { }

    public BlockedDate(Guid propertyId, DateTime date)
    {
        Id = Guid.NewGuid();
        PropertyId = propertyId;
        Date = date.Date;
    }
}
