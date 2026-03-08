using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Addresses.Create
{
    public class CreateAddressHandler
        : IRequestHandler<CreateAddressCommand, Guid>
    {
        private readonly IAddressRepository _repository;

        public CreateAddressHandler(IAddressRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(
            CreateAddressCommand request,
            CancellationToken cancellationToken)
        {
            var address = new Address(
                request.Country,
                request.City,
                request.Street,
                request.PostalCode
            );

            await _repository.AddAsync(address);

            return address.Id;
        }
    }
}