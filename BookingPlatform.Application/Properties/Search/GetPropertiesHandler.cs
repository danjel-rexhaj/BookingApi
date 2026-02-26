using BookingPlatform.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Properties.Search;

public class GetPropertiesHandler
    : IRequestHandler<GetPropertiesQuery, List<PropertyDto>>
{
    private readonly IPropertyRepository _repository;

    public GetPropertiesHandler(IPropertyRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PropertyDto>> Handle(
        GetPropertiesQuery request,
        CancellationToken cancellationToken)
    {
        var properties = await _repository.SearchAsync(
            request.City,
            request.Guests,
            request.PropertyType);

        return properties.Select(p => new PropertyDto(
            p.Id,
            p.Name,
            p.Description,
            p.Address.City,
            p.MaxGuests,
            p.PropertyType
        )).ToList();
    }
}
