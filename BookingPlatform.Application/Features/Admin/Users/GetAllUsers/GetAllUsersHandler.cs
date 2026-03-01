using BookingPlatform.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using MediatR;

namespace BookingPlatform.Application.Features.Admin.Users.GetAllUsers
{
public class GetAllUsersHandler
    : IRequestHandler<GetAllUsersQuery, List<UserDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDto>> Handle(
            GetAllUsersQuery request,
            CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                IsActive = u.IsActive
            }).ToList();
        }
    }
}
