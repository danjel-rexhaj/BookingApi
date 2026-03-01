using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Properties.BlockDate;

public class AddBlockedDateHandler
    : IRequestHandler<AddBlockedDateCommand, Guid>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IBlockedDateRepository _blockedDateRepository;
    private readonly ICurrentUserService _currentUser;

    public AddBlockedDateHandler(
        IPropertyRepository propertyRepository,
        IBlockedDateRepository blockedDateRepository,
        ICurrentUserService currentUser)
    {
        _propertyRepository = propertyRepository;
        _blockedDateRepository = blockedDateRepository;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(
        AddBlockedDateCommand request,
        CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);

        if (property == null)
            throw new Exception("Property not found");

        if (property.OwnerId != _currentUser.UserId)
            throw new Exception("Unauthorized");

        var blockedDate = new BlockedDate(
            request.PropertyId,
            request.Date
        );

        await _blockedDateRepository.AddAsync(blockedDate);

        return blockedDate.Id;
    }
}
