using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Admin.Users.ChangeRoles
{
    public class ChangeUserRoleCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string RoleName { get; set; } = null!;
    }
}
