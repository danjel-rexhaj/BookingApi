using BookingPlatform.Application.Features.Bookings.Cancel;
using BookingPlatform.Application.Features.Bookings.Confirm;
using BookingPlatform.Application.Features.Bookings.Create;
using BookingPlatform.Application.Features.Bookings.GetUserBookings;
using BookingPlatform.Application.Features.Bookings.Reject;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "Guest")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetUserBookings()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var result = await _mediator.Send(
            new GetUserBookingsQuery(Guid.Parse(userId)));

        return Ok(result);
    }


    [Authorize(Roles = "Owner")]
    [HttpPut("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        await _mediator.Send(new ConfirmBookingCommand(id));
        return NoContent();
    }

    [Authorize(Roles = "Owner")]
    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id)
    {
        await _mediator.Send(new RejectBookingCommand(id));
        return NoContent();
    }

    [Authorize] // Guest ose edhe Owner nqs e ka bere si user, por kontrolli real behet ne handler me GuestId
    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _mediator.Send(new CancelBookingCommand(id));
        return NoContent();
    }
}