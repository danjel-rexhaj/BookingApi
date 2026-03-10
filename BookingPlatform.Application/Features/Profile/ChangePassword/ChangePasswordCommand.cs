using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;


namespace BookingPlatform.Application.Features.Profile.ChangePassword;

public class ChangePasswordCommand : IRequest<Unit>
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}