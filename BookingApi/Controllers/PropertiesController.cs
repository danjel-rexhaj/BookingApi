using BookingPlatform.Application.Properties.Approve;
using BookingPlatform.Application.Properties.Create;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingPlatform.Application.Properties.Search;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PropertiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "Owner,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        await _mediator.Send(new ApprovePropertyCommand(id));
        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetPropertiesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

}
