using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Properties.Create
{
    public class CreatePropertyCommand : IRequest<Guid>
    {
        public Guid AddressId { get; set; }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string PropertyType { get; set; } = null!;

        public int MaxGuests { get; set; }

        public TimeSpan CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }
    }


}
