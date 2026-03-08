using BookingPlatform.Application.Features.Admin.Users.ChangeRoles;
using BookingPlatform.Application.Features.Admin.Users.DeleteUser;
using BookingPlatform.Application.Features.Admin.Users.GetAllUsers;
using BookingPlatform.Application.Features.Admin.Users.SuspendUser;
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());

        return Ok(result);
    }



    [HttpPut("{id}/Suspend_User")]
    public async Task<IActionResult> Suspend(Guid id)
    {
        await _mediator.Send(new SuspendUserCommand(id));
        return NoContent();
    }



    [HttpPut("{id}/Update_Role")]
    public async Task<IActionResult> ChangeRole(Guid id, ChangeUserRoleCommand command)
    {
        command.UserId = id;

        await _mediator.Send(command);

        return NoContent();
    }



    [HttpDelete("{id}/Delete_User")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }



}