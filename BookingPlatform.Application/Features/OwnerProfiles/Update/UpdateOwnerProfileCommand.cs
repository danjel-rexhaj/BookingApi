using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Features.OwnerProfiles.Update;

public record UpdateOwnerProfileCommand(
    string IdentityCardNumber,
    string BusinessName,
    string CreditCard
) : IRequest;