using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookingPlatform.Application.Features.OwnerProfiles.Get;

public class OwnerProfileDto
{
    public required string IdentityCardNumber { get; set; }
    public required string BusinessName { get; set; }
    public required string CreditCard { get; set; }
}