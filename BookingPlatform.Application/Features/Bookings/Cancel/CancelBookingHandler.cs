using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using MediatR;

namespace BookingPlatform.Application.Features.Bookings.Cancel;

public class CancelBookingHandler : IRequestHandler<CancelBookingCommand, Unit>
{
    private readonly IBookingRepository _bookingRepo;
    private readonly ICurrentUserService _currentUser;

    public CancelBookingHandler(IBookingRepository bookingRepo, ICurrentUserService currentUser)
    {
        _bookingRepo = bookingRepo;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null) throw new Exception("Booking not found.");

        // Vetem ai qe e ka bere booking (GuestId)
        if (booking.GuestId != _currentUser.UserId)
            throw new Exception("You are not allowed to cancel this booking.");

        booking.Cancel(DateTime.UtcNow);
        await _bookingRepo.SaveChangesAsync();

        return Unit.Value;
    }
}
