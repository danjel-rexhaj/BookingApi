using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using MediatR;

namespace BookingPlatform.Application.Features.Bookings.GetUserBookings;

public class GetUserBookingsHandler
    : IRequestHandler<GetUserBookingsQuery, List<GetUserBookingsResponse>>
{
    private readonly IBookingRepository _bookingRepository;

    public GetUserBookingsHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<List<GetUserBookingsResponse>> Handle(
        GetUserBookingsQuery request,
        CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository
            .GetBookingsForUserAsync(request.UserId);

        return bookings.Select(b => new GetUserBookingsResponse
        {
            Id = b.Id,
            PropertyId = b.PropertyId,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            Status = b.BookingStatus.ToString(),
            TotalPrice = b.TotalPrice,
            RefundAmount = b.RefundAmount
        }).ToList();
    }
}
