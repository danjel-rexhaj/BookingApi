using BookingPlatform.Application.Events;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Domain.Enums;
using Hangfire;
using MediatR;
using System.Text.Json;

namespace BookingPlatform.Application.Features.Reviews.Create;

public class CreateReviewHandler : IRequestHandler<CreateReviewCommand, Guid>
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IReviewRepository _reviewRepo;
    private readonly IPropertyRepository _propertyRepo;
    private readonly INotificationRepository _notificationRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly IOutboxRepository _outboxRepository;

    public CreateReviewHandler(
        IBookingRepository bookingRepo,
        IReviewRepository reviewRepo,
        IPropertyRepository propertyRepo,
        INotificationRepository notificationRepo,
        ICurrentUserService currentUser,
        INotificationService notificationService,
        IEmailTemplateService emailTemplateService,
        IEmailService emailService,
        IUserRepository userRepository,
        IOutboxRepository outboxRepository)
    {
        _bookingRepo = bookingRepo;
        _reviewRepo = reviewRepo;
        _propertyRepo = propertyRepo;
        _notificationRepo = notificationRepo;
        _currentUser = currentUser;
        _notificationService = notificationService;
        _emailTemplateService = emailTemplateService;
        _emailService = emailService;
        _userRepository = userRepository;
        _outboxRepository = outboxRepository;
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

        var alreadyReviewed = await _reviewRepo.ExistsForBookingAsync(request.BookingId);

        if (alreadyReviewed)
            throw new Exception("Booking already reviewed");

        var property = await _propertyRepo.GetByIdAsync(booking.PropertyId);

        if (property == null)
            throw new Exception("Property not found");

        var createdAtUtc = DateTime.UtcNow;

        var review = new Review(
            booking.Id,
            booking.GuestId,
            request.Rating,
            request.Comment
        );

        await _reviewRepo.AddAsync(review);

        var message = "Your property received a new review.";

        var notification = new Notification(
            property.OwnerId,
            message,
            "NewReview"
        );

        await _notificationRepo.AddAsync(notification);
        await _notificationRepo.SaveChangesAsync();

        var reviewCreatedEvent = new ReviewCreatedIntegrationEvent
        {
            ReviewId = review.Id,
            BookingId = booking.Id,
            GuestId = booking.GuestId,
            PropertyId = property.Id,
            OwnerId = property.OwnerId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAtUtc = createdAtUtc
        };

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(ReviewCreatedIntegrationEvent),
            Payload = JsonSerializer.Serialize(reviewCreatedEvent),
            OccurredOnUtc = DateTime.UtcNow
        };

        await _outboxRepository.AddAsync(outboxMessage);
        await _outboxRepository.SaveChangesAsync();

        await _notificationService.SendNotificationAsync(
            property.OwnerId,
            message
        );

        var owner = await _userRepository.GetByIdAsync(property.OwnerId);
        if (owner != null)
        {
            var emailBody = _emailTemplateService.GetNewReviewEmail(
                owner.FirstName,
                property.Name,
                request.Rating,
                request.Comment
            );

            BackgroundJob.Enqueue(() =>
                _emailService.SendEmailAsync(
                    owner.Email,
                    "Your property received a new review",
                    emailBody
                )
            );
        }

        return review.Id;
    }
}