using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Features.Auth.Login;

public record LoginQuery(
    string Email,
    string Password
) : IRequest<string>;

