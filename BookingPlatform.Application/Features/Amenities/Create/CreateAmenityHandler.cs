using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Amenities.Create
{
    public class CreateAmenityHandler
        : IRequestHandler<CreateAmenityCommand, Guid>
    {
        private readonly IAmenityRepository _repository;

        public CreateAmenityHandler(IAmenityRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(
            CreateAmenityCommand request,
            CancellationToken cancellationToken)
        {
            var amenity = new Amenity(request.Name);

            await _repository.AddAsync(amenity);

            await _repository.SaveChangesAsync();

            return amenity.Id;
        }
    }
}