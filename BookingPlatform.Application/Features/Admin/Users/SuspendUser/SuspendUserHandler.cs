using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using MediatR;


namespace BookingPlatform.Application.Features.Admin.Users.SuspendUser
{
    public class SuspendUserHandler
        : IRequestHandler<SuspendUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public SuspendUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(
            SuspendUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                throw new Exception("User not found");

            user.Deactivate();

            await _userRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
