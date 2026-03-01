using BookingPlatform.Application.Features.Auth.Login;
using BookingPlatform.Application.Features.Auth.Register;
using BookingPlatform.Application.Features.Refresh;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtService _jwtService;
    public AuthController(IMediator mediator, IJwtService jwtService)
    {
        _mediator = mediator;
        _jwtService = jwtService;
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
        var result = await _mediator.Send(query);
        return Ok(result);
    }



    [Authorize]
    [HttpGet("secure-test")]
    public IActionResult SecureTest()
    {
        return Ok("You are authenticated 🔥");
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }


    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout([FromBody] string refreshToken)
    {
        _jwtService.RemoveRefreshToken(refreshToken);
        return NoContent();
    }

}
