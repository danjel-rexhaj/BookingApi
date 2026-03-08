using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using MediatR;

namespace BookingPlatform.Application.Features.Amenities.Update
{
    public class UpdateAmenityHandler
        : IRequestHandler<UpdateAmenityCommand, Unit>
    {
        private readonly IAmenityRepository _repository;

        public UpdateAmenityHandler(IAmenityRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(
            UpdateAmenityCommand request,
            CancellationToken cancellationToken)
        {
            var amenity = await _repository.GetByIdAsync(request.Id);

            if (amenity == null)
                throw new Exception("Amenity not found");

            amenity.UpdateName(request.Name);

            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}