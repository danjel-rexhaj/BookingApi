using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingPlatform.Application.Interfaces;

namespace BookingPlatform.Infrastructure.Services;

public class EmailTemplateService : IEmailTemplateService
{
    private string BuildBaseTemplate(
        string title,
        string message,
        string? buttonText = null,
        string? buttonUrl = null)
    {
        string buttonHtml = string.Empty;

        if (!string.IsNullOrWhiteSpace(buttonText) &&
            !string.IsNullOrWhiteSpace(buttonUrl))
        {
            buttonHtml = $@"
                <div style='text-align:center; margin:30px 0;'>
                    <a href='{buttonUrl}'
                       style='background:#4f46e5; color:#ffffff; padding:14px 24px; text-decoration:none; border-radius:8px; font-weight:bold; display:inline-block;'>
                        {buttonText}
                    </a>
                </div>";
        }

        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <title>{title}</title>
        </head>
        <body style='margin:0; padding:0; background-color:#f4f4f4; font-family:Arial, sans-serif;'>
            <div style='max-width:600px; margin:40px auto; background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 4px 12px rgba(0,0,0,0.1);'>
        
                <div style='background:linear-gradient(135deg, #4f46e5, #7c3aed); color:#ffffff; padding:30px; text-align:center;'>
                    <h1 style='margin:0; font-size:28px;'>{title}</h1>
                </div>

                <div style='padding:30px; color:#333333; font-size:16px; line-height:1.6;'>
                    {message}
                    {buttonHtml}
                </div>

                <div style='background-color:#f9f9f9; padding:20px; text-align:center; font-size:12px; color:#999999;'>
                    © 2026 Booking Platform. All rights reserved.
                </div>
            </div>
        </body>
        </html>";
    }

    public string GetBookingConfirmedEmail(
    string firstName,
    string propertyName,
    DateTime startDate,
    DateTime endDate,
    decimal totalPrice)
    {
        string message = $@"
        <p>Hello {firstName},</p>
        <p>Great news. Your booking has been confirmed successfully.</p>

        <div style='background:#f9fafb; border:1px solid #e5e7eb; border-radius:10px; padding:20px; margin:25px 0;'>
            <p><strong>Property:</strong> {propertyName}</p>
            <p><strong>Check-in:</strong> {startDate:dd/MM/yyyy}</p>
            <p><strong>Check-out:</strong> {endDate:dd/MM/yyyy}</p>
            <p><strong>Total Price:</strong> €{totalPrice:0.##}</p>
        </div>

        <p>Thank you for choosing Booking Platform. We wish you a pleasant stay.</p>";

        return BuildBaseTemplate(
            "Booking Confirmed",
            message,
            "View Booking",
            "https://yourfrontendurl.com/bookings"
        );
    }

    public string GetBookingRejectedEmail(
        string firstName,
        string propertyName,
        DateTime startDate,
        DateTime endDate)
    {
        string message = $@"
        <p>Hello {firstName},</p>
        <p>We are sorry, but your booking request was rejected.</p>

        <div style='background:#f9fafb; border:1px solid #e5e7eb; border-radius:10px; padding:20px; margin:25px 0;'>
            <p><strong>Property:</strong> {propertyName}</p>
            <p><strong>Check-in:</strong> {startDate:dd/MM/yyyy}</p>
            <p><strong>Check-out:</strong> {endDate:dd/MM/yyyy}</p>
        </div>

        <p>You can explore other available properties on our platform.</p>";

        return BuildBaseTemplate(
            "Booking Rejected",
            message,
            "Browse Properties",
            "https://yourfrontendurl.com/properties"
        );
    }

    public string GetWelcomeEmail(string firstName)
    {
        string message = $@"
            <p>Hello {firstName},</p>
            <p>Your account has been created successfully.</p>
            <p>You can now explore properties, make bookings, and enjoy our platform.</p>
            <p>We are happy to have you with us.</p>";

        return BuildBaseTemplate(
            "Welcome to Booking Platform",
            message,
            "Login",
            "https://yourfrontendurl.com/login"
        );
    }

    public string GetBookingCreatedEmail(
        string firstName,
        string propertyTitle,
        DateTime startDate,
        DateTime endDate,
        int guestCount,
        decimal totalPrice)
    {
        string message = $@"
            <p>Hello {firstName},</p>
            <p>Your booking has been created successfully.</p>

            <div style='background:#f9fafb; border:1px solid #e5e7eb; border-radius:10px; padding:20px; margin:25px 0;'>
                <p><strong>Property:</strong> {propertyTitle}</p>
                <p><strong>Check-in:</strong> {startDate:dd/MM/yyyy}</p>
                <p><strong>Check-out:</strong> {endDate:dd/MM/yyyy}</p>
                <p><strong>Guests:</strong> {guestCount}</p>
                <p><strong>Total Price:</strong> €{totalPrice:0.##}</p>
            </div>

            <p>Thank you for choosing Booking Platform.</p>";

        return BuildBaseTemplate(
            "Booking Created",
            message,
            "View Booking",
            "https://yourfrontendurl.com/bookings"
        );
    }


    public string GetBookingCancelledEmail(
    string firstName,
    string propertyName,
    DateTime startDate,
    DateTime endDate)
    {
        string message = $@"
        <p>Hello {firstName},</p>
        <p>Your booking has been canceled successfully.</p>

        <div style='background:#f9fafb; border:1px solid #e5e7eb; border-radius:10px; padding:20px; margin:25px 0;'>
            <p><strong>Property:</strong> {propertyName}</p>
            <p><strong>Check-in:</strong> {startDate:dd/MM/yyyy}</p>
            <p><strong>Check-out:</strong> {endDate:dd/MM/yyyy}</p>
        </div>

        <p>If this was a mistake, you can return to the platform and create a new booking.</p>";

        return BuildBaseTemplate(
            "Booking Canceled",
            message,
            "Browse Properties",
            "https://yourfrontendurl.com/properties"
        );
    }
    public string GetBookingReminderEmail(
        string firstName,
        string propertyTitle,
        DateTime startDate)
    {
        string message = $@"
            <p>Hello {firstName},</p>
            <p>This is a friendly reminder that your booking starts tomorrow.</p>

            <div style='background:#f9fafb; border:1px solid #e5e7eb; border-radius:10px; padding:20px; margin:25px 0;'>
                <p><strong>Property:</strong> {propertyTitle}</p>
                <p><strong>Check-in:</strong> {startDate:dd/MM/yyyy}</p>
            </div>

            <p>We wish you a pleasant stay.</p>";

        return BuildBaseTemplate(
            "Booking Reminder",
            message,
            "View Booking",
            "https://yourfrontendurl.com/bookings"
        );
    }

    public string GetPasswordChangedEmail(string firstName)
    {
        string message = $@"
        <p>Hello {firstName},</p>
        <p>Your password has been changed successfully.</p>

        <div style='background:#f9fafb; border:1px solid #e5e7eb; border-radius:10px; padding:20px; margin:25px 0;'>
            <p>If you made this change, no further action is needed.</p>
            <p>If you did not make this change, please contact support immediately and secure your account.</p>
        </div>

        <p>For your security, please make sure your new password is strong and unique.</p>";

        return BuildBaseTemplate(
            "Password Changed Successfully",
            message,
            "Go to Profile",
            "https://yourfrontendurl.com/profile"
        );
    }

    public string GetOwnerProfileCreatedEmail(string firstName, string businessName)
    {
        string message = $@"
        <p>Hello {firstName},</p>
        <p>Your owner profile has been created successfully.</p>

        <div style='background:#f9fafb; border:1px solid #e5e7eb; border-radius:10px; padding:20px; margin:25px 0;'>
            <p><strong>Business Name:</strong> {businessName}</p>
        </div>

        <p>You can now create and manage your properties from your dashboard.</p>";

        return BuildBaseTemplate(
            "Welcome as an Owner",
            message,
            "Go to Dashboard",
            "https://yourfrontendurl.com/dashboard"
        );
    }

    public string GetPropertyApprovedEmail(
        string firstName,
        string propertyName,
        string city,
        string country,
        string propertyType,
        int maxGuests,
        decimal pricePerNight)
    {
        string message = $@"
        <p>Hello {firstName},</p>
        <p>Good news. Your property has been approved successfully.</p>

        <div style='background:#f9fafb; border:1px solid #e5e7eb; border-radius:10px; padding:20px; margin:25px 0;'>
            <p><strong>Property:</strong> {propertyName}</p>
            <p><strong>City:</strong> {city}</p>
            <p><strong>Country:</strong> {country}</p>
            <p><strong>Type:</strong> {propertyType}</p>
            <p><strong>Max Guests:</strong> {maxGuests}</p>
            <p><strong>Price per Night:</strong> €{pricePerNight:0.##}</p>
        </div>

        <p>Your listing is now active and visible on the platform.</p>";

        return BuildBaseTemplate(
            "Property Approved",
            message,
            "Go to Dashboard",
            "https://yourfrontendurl.com/dashboard"
        );
    }

    public string GetPropertyRejectedEmail(
        string firstName,
        string propertyName,
        string city,
        string country,
        string propertyType,
        int maxGuests,
        decimal pricePerNight)
    {
        string message = $@"
        <p>Hello {firstName},</p>
        <p>We are sorry to inform you that your property has been rejected.</p>

        <div style='background:#f9fafb; border:1px solid #e5e7eb; border-radius:10px; padding:20px; margin:25px 0;'>
            <p><strong>Property:</strong> {propertyName}</p>
            <p><strong>City:</strong> {city}</p>
            <p><strong>Country:</strong> {country}</p>
            <p><strong>Type:</strong> {propertyType}</p>
            <p><strong>Max Guests:</strong> {maxGuests}</p>
            <p><strong>Price per Night:</strong> €{pricePerNight:0.##}</p>
        </div>

        <p>Please review your property details and update the listing if needed.</p>";

        return BuildBaseTemplate(
            "Property Rejected",
            message,
            "Go to Dashboard",
            "https://yourfrontendurl.com/dashboard"
        );
    }

    public string GetNewReviewEmail(string firstName, string propertyName, int rating, string comment)
    {
        string message = $@"
        <p>Hello {firstName},</p>
        <p>Your property has received a new review.</p>

        <div style='background:#f9fafb; border:1px solid #e5e7eb; border-radius:10px; padding:20px; margin:25px 0;'>
            <p><strong>Property:</strong> {propertyName}</p>
            <p><strong>Rating:</strong> {rating}/5</p>
            <p><strong>Comment:</strong> {comment}</p>
        </div>

        <p>You can check your dashboard to see more details.</p>";

        return BuildBaseTemplate(
            "New Review Received",
            message,
            "Go to Dashboard",
            "https://yourfrontendurl.com/dashboard"
        );
    }

    public string GetNewBookingRequestEmail(
        string firstName,
        string propertyName,
        DateTime startDate,
        DateTime endDate,
        int guestCount,
        decimal totalPrice)
    {
        string message = $@"
        <p>Hello {firstName},</p>
        <p>You have received a new booking request for your property.</p>

        <div style='background:#f9fafb; border:1px solid #e5e7eb; border-radius:10px; padding:20px; margin:25px 0;'>
            <p><strong>Property:</strong> {propertyName}</p>
            <p><strong>Check-in:</strong> {startDate:dd/MM/yyyy}</p>
            <p><strong>Check-out:</strong> {endDate:dd/MM/yyyy}</p>
            <p><strong>Guests:</strong> {guestCount}</p>
            <p><strong>Total Price:</strong> €{totalPrice:0.##}</p>
        </div>

        <p>Please review this booking request from your dashboard.</p>";

        return BuildBaseTemplate(
            "New Booking Request",
            message,
            "Go to Dashboard",
            "https://yourfrontendurl.com/dashboard"
        );
    }

    public string GetPropertySuspendedEmail(
        string firstName,
        string propertyName,
        string city,
        string country,
        string propertyType,
        int maxGuests,
        decimal pricePerNight)
    {
        string message = $@"
            <p>Hello {firstName},</p>
            <p>We would like to inform you that your property has been suspended.</p>

            <div style='background:#f9fafb; border:1px solid #e5e7eb; border-radius:10px; padding:20px; margin:25px 0;'>
                <p><strong>Property:</strong> {propertyName}</p>
                <p><strong>City:</strong> {city}</p>
                <p><strong>Country:</strong> {country}</p>
                <p><strong>Type:</strong> {propertyType}</p>
                <p><strong>Max Guests:</strong> {maxGuests}</p>
                <p><strong>Price per Night:</strong> €{pricePerNight:0.##}</p>
            </div>

            <p>Please review your listing or contact support for more information.</p>";

        return BuildBaseTemplate(
            "Property Suspended",
            message,
            "Go to Dashboard",
            "https://yourfrontendurl.com/dashboard");
    }

}