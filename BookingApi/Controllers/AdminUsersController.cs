using BookingPlatform.Application.Features.Admin.Users.GetAllUsers;
using BookingPlatform.Application.Features.Admin.Users.SuspendUser;
using BookingPlatform.Application.Features.Admin.Users.DeleteUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.API.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminUsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());
        return Ok(result);
    }

    [HttpPut("{id}/suspend")]
    public async Task<IActionResult> Suspend(Guid id)
    {
        await _mediator.Send(new SuspendUserCommand(id));
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }
}