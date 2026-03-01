using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookingPlatform.Application.Features.Bookings.GetUserBookings;

public class GetUserBookingsResponse
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public decimal? RefundAmount { get; set; }
}
