using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Interfaces;

public interface IEmailTemplateService
{
    string GetWelcomeEmail(string firstName);

    string GetBookingCreatedEmail(
        string firstName,
        string propertyName,
        DateTime startDate,
        DateTime endDate,
        int guestCount,
        decimal totalPrice
    );

    string GetBookingReminderEmail(
        string firstName,
        string propertyName,
        DateTime startDate
    );

    string GetBookingConfirmedEmail(
        string firstName,
        string propertyName,
        DateTime startDate,
        DateTime endDate,
        decimal totalPrice
    );

    string GetBookingRejectedEmail(
        string firstName,
        string propertyName,
        DateTime startDate,
        DateTime endDate
    );

    string GetBookingCancelledEmail(
        string firstName,
        string propertyName,
        DateTime startDate,
        DateTime endDate
    );

    string GetPasswordChangedEmail
        (string firstName);

    string GetOwnerProfileCreatedEmail(
        string firstName,
        string businessName);

    string GetPropertyApprovedEmail(
        string firstName,
        string propertyName,
        string city,
        string country,
        string propertyType,
        int maxGuests,
        decimal pricePerNight
    );

    string GetPropertyRejectedEmail(
        string firstName,
        string propertyName,
        string city,
        string country,
        string propertyType,
        int maxGuests,
        decimal pricePerNight
    );

    string GetNewReviewEmail(
        string firstName,
        string propertyName,
        int rating,
        string comment);

    string GetNewBookingRequestEmail(
        string firstName,
        string propertyName,
        DateTime startDate,
        DateTime endDate,
        int guestCount,
        decimal totalPrice);

    string GetPropertySuspendedEmail(
        string firstName,
        string propertyName,
        string city,
        string country,
        string propertyType,
        int maxGuests,
        decimal pricePerNight);
}