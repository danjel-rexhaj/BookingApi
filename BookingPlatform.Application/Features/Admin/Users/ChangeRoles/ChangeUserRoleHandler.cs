using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Admin.Users.ChangeRoles
{
    public class ChangeUserRoleHandler : IRequestHandler<ChangeUserRoleCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public ChangeUserRoleHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<Unit> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                throw new Exception("User not found");

            var role = await _roleRepository.GetByNameAsync(request.RoleName);

            if (role == null)
                throw new Exception("Role not found");

            user.UserRoles.Clear();

            user.UserRoles.Add(new UserRole(user.Id, role.Id, DateTime.UtcNow));

            await _userRepository.UpdateAsync(user);

            return Unit.Value;
        }
    }
}
