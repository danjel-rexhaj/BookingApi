using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Bookings.Cancel;

public class CancelBookingHandler : IRequestHandler<CancelBookingCommand, Unit>
{
    private readonly IBookingRepository _bookingRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notificationRepository;

    public CancelBookingHandler(IBookingRepository bookingRepo,
        ICurrentUserService currentUser,
        INotificationRepository notificationRepository)
    {
        _bookingRepo = bookingRepo;
        _currentUser = currentUser;
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null) throw new Exception("Booking not found.");

        if (booking.GuestId != _currentUser.UserId)
            throw new Exception("You are not allowed to cancel this booking.");

        booking.CancelWithPolicy(); 

        await _bookingRepo.SaveChangesAsync();

        var notification = new Notification(
            booking.GuestId,
            "Your booking has been canceled.",
            "BookingCanceled");


        await _notificationRepository.AddAsync(notification);
        await _bookingRepo.SaveChangesAsync();

        return Unit.Value;
    }
}
