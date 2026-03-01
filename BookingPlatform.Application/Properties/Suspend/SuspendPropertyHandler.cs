using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using BookingPlatform.Application.Interfaces;

namespace BookingPlatform.Application.Properties.Suspend;

public class SuspendPropertyHandler
    : IRequestHandler<SuspendPropertyCommand>
{
    private readonly IPropertyRepository _repository;

    public SuspendPropertyHandler(IPropertyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(
        SuspendPropertyCommand request,
        CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.PropertyId);

        if (property == null)
            throw new Exception("Property not found");

        property.Suspend();

        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}
