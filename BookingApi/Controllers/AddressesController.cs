using BookingPlatform.Application.Features.Addresses.Create;
using BookingPlatform.Application.Features.Addresses.Update;
using BookingPlatform.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/addresses")]
    [Authorize(Roles = "Owner,Admin")]
    public class AddressesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAddressRepository _repository;

        public AddressesController(IMediator mediator, IAddressRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(CreateAddressCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var addresses = await _repository.GetAllAsync();
            return Ok(addresses);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateAddressCommand command)
        {
            command = command with { Id = id };

            await _mediator.Send(command);

            return NoContent();
        }
    }
}