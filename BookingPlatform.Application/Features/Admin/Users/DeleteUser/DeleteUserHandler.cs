using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using MediatR;



namespace BookingPlatform.Application.Features.Admin.Users.DeleteUser
{
    public class DeleteUserHandler
        : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(
            DeleteUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                throw new Exception("User not found");

            await _userRepository.DeleteAsync(user);
            await _userRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
