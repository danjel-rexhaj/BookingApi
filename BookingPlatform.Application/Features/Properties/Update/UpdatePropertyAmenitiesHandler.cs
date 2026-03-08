using BookingPlatform.Domain.Entities;
using MediatR;
using System;
using BookingPlatform.Application.Interfaces;


namespace BookingPlatform.Application.Features.Properties.Update;

public class UpdatePropertyAmenitiesHandler
    : IRequestHandler<UpdatePropertyAmenitiesCommand, Unit>
{
    private readonly IPropertyRepository _repository;

    public UpdatePropertyAmenitiesHandler(IPropertyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdatePropertyAmenitiesCommand request, CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.PropertyId);

        if (property == null)
            throw new Exception("Property not found");

        property.UpdateAmenities(request.AmenityIds);

        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}