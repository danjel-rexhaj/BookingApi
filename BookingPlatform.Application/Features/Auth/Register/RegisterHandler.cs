using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using MediatR;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Features.Auth.Register;

public class RegisterHandler
    : IRequestHandler<RegisterCommand, string>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRoleRepository _roleRepository;
    private readonly IEmailService _emailService;
    public RegisterHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    public async Task<string> Handle(
     RegisterCommand request,
     CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository
            .GetByEmailAsync(request.Email);

        if (existingUser != null)
            throw new Exception("User already exists");

        var hashedPassword = _passwordHasher.Hash(request.Password);

        var user = new User(
            request.FirstName,
            request.LastName,
            request.Email,
            hashedPassword
        );

        var guestRole = await _roleRepository.GetByNameAsync("Guest");

        if (guestRole == null)
            throw new Exception("Guest role not found");

        user.UserRoles.Add(new UserRole(
            user.Id,
            guestRole.Id,
            DateTime.UtcNow
        ));

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        BackgroundJob.Enqueue(() =>
            _emailService.SendEmailAsync(
                user.Email,
                "Welcome to Booking Platform",
                "Welcome to our platform! Your account has been created successfully."
            )
        );

        return "User registered successfully";
    }

}
