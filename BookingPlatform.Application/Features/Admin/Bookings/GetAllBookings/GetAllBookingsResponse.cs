using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace BookingPlatform.Application.Features.Admin.Bookings.GetAllBookings;

public class GetAllBookingsResponse
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public Guid GuestId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}
