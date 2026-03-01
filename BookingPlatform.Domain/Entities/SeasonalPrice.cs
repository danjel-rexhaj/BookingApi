using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Domain.Entities
{
    public class SeasonalPrice
    {
        public Guid Id { get; private set; }
        public Guid PropertyId { get; private set; }

        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public decimal PricePerNight { get; private set; }

        private SeasonalPrice() { }

        public SeasonalPrice(Guid propertyId, DateTime startDate, DateTime endDate, decimal pricePerNight)
        {
            Id = Guid.NewGuid();
            PropertyId = propertyId;
            StartDate = startDate;
            EndDate = endDate;
            PricePerNight = pricePerNight;
        }


    }
}
