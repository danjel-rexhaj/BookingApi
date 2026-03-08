using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingPlatform.Application.Features.OwnerProfiles.Create;
using BookingPlatform.Application.Features.OwnerProfiles.Update;
using BookingPlatform.Application.Features.OwnerProfiles.Get;
namespace BookingPlatform.API.Controllers;

[ApiController]
[Route("api/owner-profile")]
[Authorize]
public class OwnerProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public OwnerProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOwnerProfileCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateOwnerProfileCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator.Send(new GetOwnerProfileQuery());

        return Ok(result);
    }
}