using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Features.Addresses.Update
{
    public record UpdateAddressCommand(
        Guid Id,
        string Country,
        string City,
        string Street,
        string PostalCode
    ) : IRequest<Unit>;
}