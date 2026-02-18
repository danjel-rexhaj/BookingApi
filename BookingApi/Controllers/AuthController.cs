using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;

using BookingPlatform.Application.Features.Auth.Register;
using BookingPlatform.Application.Features.Auth.Login;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginQuery query)
    {
        var token = await _mediator.Send(query);
        return Ok(token);
    }

    [Authorize]
    [HttpGet("secure-test")]
    public IActionResult SecureTest()
    {
        return Ok("You are authenticated 🔥");
    }
}
