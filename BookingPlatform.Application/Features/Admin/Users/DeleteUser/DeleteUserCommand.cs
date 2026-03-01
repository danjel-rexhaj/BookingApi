using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Features.Admin.Users.DeleteUser;

public class DeleteUserCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }

    public DeleteUserCommand(Guid userId)
    {
        UserId = userId;
    }
}