using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.API.Controllers;

[ApiController]
[Route("api/properties/{propertyId}/images")]
public class PropertyImagesController : ControllerBase
{
    private readonly IPropertyImageRepository _repository;

    public PropertyImagesController(IPropertyImageRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Add(Guid propertyId, [FromBody] string imageUrl)
    {
        var image = new PropertyImage(propertyId, imageUrl);

        await _repository.AddAsync(image);
        await _repository.SaveChangesAsync();

        return Ok(image);
    }
}