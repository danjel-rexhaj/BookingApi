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
//data transfer object DTO , nuk perdorim direkt nga entities sepse nuk duam te ekspozojme direkt domain entities ne API 
//, dhe gjithashtu mund te kemi nevoje te transformojme apo te perzgjedhim vetem disa nga fushat e entity-ve per t'i derguar ne klient.