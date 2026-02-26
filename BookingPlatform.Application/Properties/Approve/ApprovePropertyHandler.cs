using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;

namespace BookingPlatform.Application.Properties.Approve
{
    public class ApprovePropertyHandler : IRequestHandler<ApprovePropertyCommand>
    {
        private readonly IPropertyRepository _repository;

        public ApprovePropertyHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(ApprovePropertyCommand request, CancellationToken cancellationToken)
        {
            var property = await _repository.GetByIdAsync(request.PropertyId);

            if (property == null)
                throw new Exception("Property not found");

            property.Approve();   // do ta shtojmë këtë metodë në entity

            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
