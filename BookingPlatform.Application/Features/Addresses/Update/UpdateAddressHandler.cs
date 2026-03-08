using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using MediatR;

namespace BookingPlatform.Application.Features.Addresses.Update
{
    public class UpdateAddressHandler
        : IRequestHandler<UpdateAddressCommand, Unit>
    {
        private readonly IAddressRepository _repository;

        public UpdateAddressHandler(IAddressRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(
            UpdateAddressCommand request,
            CancellationToken cancellationToken)
        {
            var address = await _repository.GetByIdAsync(request.Id);

            if (address == null)
                throw new Exception("Address not found");

            address.Update(
                request.Country,
                request.City,
                request.Street,
                request.PostalCode
            );

            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}