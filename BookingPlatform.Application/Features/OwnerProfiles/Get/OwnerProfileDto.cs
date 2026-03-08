using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookingPlatform.Application.Features.OwnerProfiles.Get;

public class OwnerProfileDto
{
    public string IdentityCardNumber { get; set; }
    public string BusinessName { get; set; }
    public string CreditCard { get; set; }
}