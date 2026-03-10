using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Bookings.Reject;

public class RejectBookingHandler : IRequestHandler<RejectBookingCommand, Unit>
{
    private readonly IBookingRepository _bookingRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notificationRepository;

    public RejectBookingHandler(IBookingRepository bookingRepo, 
        ICurrentUserService currentUser,
        INotificationRepository notificationRepository)
    {
        _bookingRepo = bookingRepo;
        _currentUser = currentUser;
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(RejectBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null) throw new Exception("Booking not found.");

        if (booking.Property.OwnerId != _currentUser.UserId)
            throw new Exception("You are not allowed to reject this booking.");

        booking.Reject(DateTime.UtcNow);
        await _bookingRepo.SaveChangesAsync();

        var notification = new Notification(
            booking.GuestId,
            "Your booking has been rejected.",
            "BookingRejected");


        await _notificationRepository.AddAsync(notification);
        await _bookingRepo.SaveChangesAsync();

        return Unit.Value;
    }
}
