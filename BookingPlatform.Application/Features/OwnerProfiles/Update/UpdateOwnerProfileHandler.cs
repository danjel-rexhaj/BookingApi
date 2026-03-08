using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using MediatR;

namespace BookingPlatform.Application.Features.OwnerProfiles.Update;

public class UpdateOwnerProfileHandler
    : IRequestHandler<UpdateOwnerProfileCommand>
{
    private readonly IOwnerProfileRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public UpdateOwnerProfileHandler(
        IOwnerProfileRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        UpdateOwnerProfileCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var profile = await _repository.GetByUserIdAsync(userId);

        if (profile == null)
            throw new Exception("Owner profile not found");

        profile.Update(
            request.IdentityCardNumber,
            request.BusinessName,
            request.CreditCard
        );

        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}