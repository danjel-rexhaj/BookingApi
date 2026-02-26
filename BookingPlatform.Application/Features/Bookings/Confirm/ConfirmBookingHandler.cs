using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;
using MediatR;

namespace BookingPlatform.Application.Features.Bookings.Confirm;

public class ConfirmBookingHandler : IRequestHandler<ConfirmBookingCommand, Unit>
{
    private readonly IBookingRepository _bookingRepo;
    private readonly ICurrentUserService _currentUser;

    public ConfirmBookingHandler(IBookingRepository bookingRepo, ICurrentUserService currentUser)
    {
        _bookingRepo = bookingRepo;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null) throw new Exception("Booking not found.");

        // Vetem Owner i property mund ta konfirmoje
        if (booking.Property.OwnerId != _currentUser.UserId)
            throw new Exception("You are not allowed to confirm this booking.");

        booking.Confirm(DateTime.UtcNow);
        await _bookingRepo.SaveChangesAsync();

        return Unit.Value;
    }
}
