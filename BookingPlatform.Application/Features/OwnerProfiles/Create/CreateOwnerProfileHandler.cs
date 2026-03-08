using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.OwnerProfiles.Create;

public class CreateOwnerProfileHandler
    : IRequestHandler<CreateOwnerProfileCommand>
{
    private readonly IOwnerProfileRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CreateOwnerProfileHandler(
        IOwnerProfileRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        CreateOwnerProfileCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var profile = new OwnerProfile(
            userId,
            request.IdentityCardNumber,
            request.BusinessName,
            request.CreditCard
        );

        await _repository.AddAsync(profile);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}