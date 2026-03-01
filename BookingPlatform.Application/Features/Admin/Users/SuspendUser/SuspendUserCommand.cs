using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Features.Admin.Users.SuspendUser;

public class SuspendUserCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }

    public SuspendUserCommand(Guid userId)
    {
        UserId = userId;
    }
}