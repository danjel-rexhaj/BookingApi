using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BookingPlatform.Application.Features.Properties.Create;

public record CreatePropertyCommand(
    Guid AddressId,
    string Name,
    string Description,
    string PropertyType,
    int MaxGuests,
    TimeSpan CheckInTime,
    TimeSpan CheckOutTime,
    decimal BasePricePerNight,
    decimal CleaningFee,
    decimal ServiceFee,
    decimal TaxPercentage,
    decimal AdditionalGuestFee,
    int? MinimumStay,
    int? MaximumStay,
    List<string> Rules
) : IRequest<Guid>;