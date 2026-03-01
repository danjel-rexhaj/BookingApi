using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Domain.Enums;
using MediatR;

namespace BookingPlatform.Application.Features.Reviews.Create;

public class CreateReviewHandler
    : IRequestHandler<CreateReviewCommand, Guid>
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IReviewRepository _reviewRepo;
    private readonly IPropertyRepository _propertyRepo;
    private readonly INotificationRepository _notificationRepo;
    private readonly ICurrentUserService _currentUser;

    public CreateReviewHandler(
        IBookingRepository bookingRepo,
        IReviewRepository reviewRepo,
        IPropertyRepository propertyRepo,
        INotificationRepository notificationRepo,
        ICurrentUserService currentUser)
    {
        _bookingRepo = bookingRepo;
        _reviewRepo = reviewRepo;
        _propertyRepo = propertyRepo;
        _notificationRepo = notificationRepo;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(
        CreateReviewCommand request,
        CancellationToken cancellationToken)
    {
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);

        if (booking == null)
            throw new Exception("Booking not found");

        if (booking.GuestId != _currentUser.UserId)
            throw new Exception("You can review only your booking");

        if (booking.BookingStatus != BookingStatus.Completed)
            throw new Exception("You can review only completed bookings");

        var alreadyReviewed =
            await _reviewRepo.ExistsForBookingAsync(request.BookingId);

        if (alreadyReviewed)
            throw new Exception("Booking already reviewed");

        // 🔹 Create Review
        var review = new Review(
            booking.Id,
            booking.GuestId,
            request.Rating,
            request.Comment);

        await _reviewRepo.AddAsync(review);

        // 🔹 Load Property (mos përdor navigation property)
        var property = await _propertyRepo.GetByIdAsync(booking.PropertyId);

        if (property == null)
            throw new Exception("Property not found");

        // 🔹 Create Notification for Owner
        var notification = new Notification(
            property.OwnerId,
            "Your property received a new review.",
            "NewReview");

        await _notificationRepo.AddAsync(notification);
        await _notificationRepo.SaveChangesAsync();

        return review.Id;
    }
}