using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Refresh
{
    public record RefreshTokenCommand(string RefreshToken)
    : IRequest<AuthResponseDto>;
}
