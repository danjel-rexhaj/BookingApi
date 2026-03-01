using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using BookingPlatform.Application.Interfaces;

namespace BookingPlatform.Application.Features.Admin.Bookings.GetAllBookings;

public class GetAllBookingsHandler
    : IRequestHandler<GetAllBookingsQuery, List<GetAllBookingsResponse>>
{
    private readonly IBookingRepository _repository;

    public GetAllBookingsHandler(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GetAllBookingsResponse>> Handle(
        GetAllBookingsQuery request,
        CancellationToken cancellationToken)
    {
        var bookings = await _repository.GetAllAsync();

        return bookings.Select(b => new GetAllBookingsResponse
        {
            Id = b.Id,
            PropertyId = b.PropertyId,
            GuestId = b.GuestId,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            Status = b.BookingStatus.ToString(),
            TotalPrice = b.TotalPrice
        }).ToList();
    }
}
