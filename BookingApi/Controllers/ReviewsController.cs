using BookingPlatform.Application.Features.Reviews.Create;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "Guest")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateReviewCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }
}