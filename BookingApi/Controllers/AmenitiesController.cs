using BookingPlatform.Application.Features.Amenities.Create;
using BookingPlatform.Application.Features.Amenities.Update;
using BookingPlatform.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/amenities")]
    [Authorize(Roles = "Owner,Admin")]
    public class AmenitiesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAmenityRepository _repository;

        public AmenitiesController(IMediator mediator, IAmenityRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(CreateAmenityCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var amenities = await _repository.GetAllAsync();
            return Ok(amenities);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateAmenityCommand command)
        {
            command = command with { Id = id };

            await _mediator.Send(command);

            return NoContent();
        }
    }
}