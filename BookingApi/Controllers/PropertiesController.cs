using BookingPlatform.Application.Features.Properties.Approve;
using BookingPlatform.Application.Features.Properties.BlockDate;
using BookingPlatform.Application.Features.Properties.Create;
using BookingPlatform.Application.Features.Properties.Reject;
using BookingPlatform.Application.Features.Properties.Search;
using BookingPlatform.Application.Features.Properties.SeasonalPrice;
using BookingPlatform.Application.Features.Properties.Suspend;
using BookingPlatform.Application.Features.Properties.Update;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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





    [HttpPut("{id}/suspend")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Suspend(Guid id)
    {
        await _mediator.Send(new SuspendPropertyCommand(id));
        return NoContent();
    }






    [HttpPost("{id}/seasonal-price")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> AddSeasonalPrice(
    Guid id,
    [FromBody] AddSeasonalPriceCommand command)
    {
        var result = await _mediator.Send(
            command with { PropertyId = id });

        return Ok(result);
    }




    [HttpPost("{id}/block-date")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> BlockDate(
    Guid id,
    [FromBody] AddBlockedDateCommand command)
    {
        var result = await _mediator.Send(
            command with { PropertyId = id });

        return Ok(result);
    }
    



    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(Guid id)
    {
        await _mediator.Send(new RejectPropertyCommand(id));
        return NoContent();
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProperty(
    Guid id,
    UpdatePropertyCommand command)
    {
        if (id != command.PropertyId)
            return BadRequest();

        await _mediator.Send(command);

        return NoContent();
    }

}
