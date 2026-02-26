using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using MediatR;

namespace BookingPlatform.Application.Features.Bookings.Reject;

public class RejectBookingHandler : IRequestHandler<RejectBookingCommand, Unit>
{
    private readonly IBookingRepository _bookingRepo;
    private readonly ICurrentUserService _currentUser;

    public RejectBookingHandler(IBookingRepository bookingRepo, ICurrentUserService currentUser)
    {
        _bookingRepo = bookingRepo;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(RejectBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null) throw new Exception("Booking not found.");

        if (booking.Property.OwnerId != _currentUser.UserId)
            throw new Exception("You are not allowed to reject this booking.");

        booking.Reject(DateTime.UtcNow);
        await _bookingRepo.SaveChangesAsync();

        return Unit.Value;
    }
}
