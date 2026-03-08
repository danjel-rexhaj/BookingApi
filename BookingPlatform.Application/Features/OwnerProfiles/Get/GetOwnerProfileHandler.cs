using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using MediatR;

namespace BookingPlatform.Application.Features.OwnerProfiles.Get;

public class GetOwnerProfileHandler
    : IRequestHandler<GetOwnerProfileQuery, OwnerProfileDto>
{
    private readonly IOwnerProfileRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetOwnerProfileHandler(
        IOwnerProfileRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<OwnerProfileDto> Handle(
        GetOwnerProfileQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var profile = await _repository.GetByUserIdAsync(userId);

        if (profile == null)
            throw new Exception("Owner profile not found");

        return new OwnerProfileDto
        {
            IdentityCardNumber = profile.IdentityCardNumber,   
            BusinessName = profile.BusinessName,
            CreditCard = profile.CreditCard
        };
    }
}