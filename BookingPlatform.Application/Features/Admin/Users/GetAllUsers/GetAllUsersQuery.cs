using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Features.Admin.Users.GetAllUsers;

public class GetAllUsersQuery
    : IRequest<List<UserDto>>
{
}
