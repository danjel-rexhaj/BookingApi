using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Bookings.Confirm;

public class ConfirmBookingHandler : IRequestHandler<ConfirmBookingCommand, Unit>
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IPropertyRepository _propertyRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notificationRepository;
    public ConfirmBookingHandler(
        IBookingRepository bookingRepo,
        IPropertyRepository propertyRepo,
        ICurrentUserService currentUser,
        INotificationRepository notificationRepository)
    {
        _bookingRepo = bookingRepo;
        _propertyRepo = propertyRepo;
        _currentUser = currentUser;
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(
        ConfirmBookingCommand request,
        CancellationToken cancellationToken)
    {
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null)
            throw new Exception("Booking not found.");

        var property = await _propertyRepo.GetByIdAsync(booking.PropertyId);
        if (property == null)
            throw new Exception("Property not found.");

        if (property.OwnerId != _currentUser.UserId)
            throw new Exception("You are not allowed to confirm this booking.");

        booking.Confirm(DateTime.UtcNow);

        await _bookingRepo.SaveChangesAsync();

        var notification = new Notification(
            booking.GuestId,
            "Your booking has been confirmed.",
            "BookingConfirmed");

        await _notificationRepository.AddAsync(notification);
        await _notificationRepository.SaveChangesAsync();

        return Unit.Value;
    }
}