using BookingPlatform.Application.Features.Profile.UpdateProfile;
using BookingPlatform.Application.Features.Profile.ChangePassword;
using BookingPlatform.Application.Features.Profile.UpdateProfileImage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.API.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(UpdateProfileCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPut("image")]
    public async Task<IActionResult> UpdateImage(UpdateProfileImageCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }
}