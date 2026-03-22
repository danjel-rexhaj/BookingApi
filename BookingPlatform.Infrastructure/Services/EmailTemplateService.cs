using System;
using BookingPlatform.Application.Interfaces;

namespace BookingPlatform.Infrastructure.Services;

public class EmailTemplateService : IEmailTemplateService
{
    private string BuildBaseTemplate(
        string title,
        string previewText,
        string message)
    {
        return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>{title}</title>
</head>
<body style='margin:0; padding:0; background-color:#f5f7fb; font-family:Arial, Helvetica, sans-serif; color:#1f2937;'>
    <div style='display:none; max-height:0; overflow:hidden; opacity:0;'>
        {previewText}
    </div>

    <table role='presentation' cellpadding='0' cellspacing='0' border='0' width='100%' style='background-color:#f5f7fb; margin:0; padding:24px 12px;'>
        <tr>
            <td align='center'>
                <table role='presentation' cellpadding='0' cellspacing='0' border='0' width='100%' style='max-width:600px; background-color:#ffffff; border:1px solid #e5e7eb; border-radius:14px; overflow:hidden;'>
                    <tr>
                        <td style='padding:24px 28px 12px 28px; border-bottom:1px solid #eef2f7;'>
                            <div style='font-size:13px; font-weight:700; letter-spacing:0.4px; color:#2563eb; text-transform:uppercase;'>
                                AlBooking
                            </div>
                            <h1 style='margin:10px 0 0 0; font-size:24px; line-height:1.3; color:#111827; font-weight:700;'>
                                {title}
                            </h1>
                        </td>
                    </tr>

                    <tr>
                        <td style='padding:28px; font-size:15px; line-height:1.7; color:#374151;'>
                            {message}
                        </td>
                    </tr>

                    <tr>
                        <td style='padding:18px 28px; border-top:1px solid #eef2f7; background-color:#fafbfc; font-size:12px; line-height:1.6; color:#6b7280; text-align:center;'>
                            This is an automated email from AlBooking. For support, contact support@albooking.online.
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private string BuildInfoCard(params (string Label, string Value)[] rows)
    {
        var html = "<div style='margin:20px 0; padding:18px 20px; background-color:#f9fafb; border:1px solid #e5e7eb; border-radius:12px;'>";

        foreach (var row in rows)
        {
            html += $@"
                <p style='margin:0 0 10px 0; font-size:14px; color:#374151;'>
                    <span style='font-weight:700; color:#111827;'>{row.Label}</span> {row.Value}
                </p>";
        }

        html += "</div>";
        return html;
    }

    public string GetBookingConfirmedEmail(
        string firstName,
        string propertyName,
        DateTime startDate,
        DateTime endDate,
        decimal totalPrice)
    {
        string details = BuildInfoCard(
            ("Property:", propertyName),
            ("Check-in:", startDate.ToString("dd/MM/yyyy")),
            ("Check-out:", endDate.ToString("dd/MM/yyyy")),
            ("Total price:", $"€{totalPrice:0.##}")
        );

        string message = $@"
            <p style='margin-top:0;'>Hello {firstName},</p>
            <p>Your booking has been confirmed successfully.</p>
            {details}
            <p>Thank you for choosing AlBooking. We wish you a pleasant stay.</p>";

        return BuildBaseTemplate(
            "Booking confirmed",
            "Your booking has been confirmed successfully.",
            message
        );
    }

    public string GetBookingRejectedEmail(
        string firstName,
        string propertyName,
        DateTime startDate,
        DateTime endDate)
    {
        string details = BuildInfoCard(
            ("Property:", propertyName),
            ("Check-in:", startDate.ToString("dd/MM/yyyy")),
            ("Check-out:", endDate.ToString("dd/MM/yyyy"))
        );

        string message = $@"
            <p style='margin-top:0;'>Hello {firstName},</p>
            <p>We’re sorry, but your booking request could not be approved.</p>
            {details}
            <p>You can explore other available properties on AlBooking.</p>";

        return BuildBaseTemplate(
            "Booking request declined",
            "Your booking request could not be approved.",
            message
        );
    }

    public string GetWelcomeEmail(string firstName)
    {
        string message = $@"
            <p style='margin-top:0;'>Hello {firstName},</p>
            <p>Your account has been created successfully.</p>
            <p>You can now explore properties, create bookings, and manage your activity on AlBooking.</p>
            <p>We’re happy to have you with us.</p>";

        return BuildBaseTemplate(
            "Welcome to AlBooking",
            "Your AlBooking account has been created successfully.",
            message
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
        string details = BuildInfoCard(
            ("Property:", propertyTitle),
            ("Check-in:", startDate.ToString("dd/MM/yyyy")),
            ("Check-out:", endDate.ToString("dd/MM/yyyy")),
            ("Guests:", guestCount.ToString()),
            ("Total price:", $"€{totalPrice:0.##}")
        );

        string message = $@"
            <p style='margin-top:0;'>Hello {firstName},</p>
            <p>We have received your booking request successfully.</p>
            {details}
            <p>You can review your booking details in your account.</p>";

        return BuildBaseTemplate(
            "Booking request received",
            "We have received your booking request successfully.",
            message
        );
    }

    public string GetBookingCancelledEmail(
        string firstName,
        string propertyName,
        DateTime startDate,
        DateTime endDate)
    {
        string details = BuildInfoCard(
            ("Property:", propertyName),
            ("Check-in:", startDate.ToString("dd/MM/yyyy")),
            ("Check-out:", endDate.ToString("dd/MM/yyyy"))
        );

        string message = $@"
            <p style='margin-top:0;'>Hello {firstName},</p>
            <p>Your booking has been cancelled successfully.</p>
            {details}
            <p>If this was a mistake, you can return later and create a new booking.</p>";

        return BuildBaseTemplate(
            "Booking cancelled",
            "Your booking has been cancelled successfully.",
            message
        );
    }

    public string GetBookingReminderEmail(
        string firstName,
        string propertyTitle,
        DateTime startDate)
    {
        string details = BuildInfoCard(
            ("Property:", propertyTitle),
            ("Check-in:", startDate.ToString("dd/MM/yyyy"))
        );

        string message = $@"
            <p style='margin-top:0;'>Hello {firstName},</p>
            <p>This is a friendly reminder that your booking starts tomorrow.</p>
            {details}
            <p>We wish you a pleasant stay.</p>";

        return BuildBaseTemplate(
            "Booking reminder",
            "Reminder: your booking starts tomorrow.",
            message
        );
    }

    public string GetPasswordChangedEmail(string firstName)
    {
        string message = $@"
            <p style='margin-top:0;'>Hello {firstName},</p>
            <p>Your password has been changed successfully.</p>
            <div style='margin:20px 0; padding:18px 20px; background-color:#f9fafb; border:1px solid #e5e7eb; border-radius:12px;'>
                <p style='margin:0 0 10px 0;'>If you made this change, no further action is needed.</p>
                <p style='margin:0;'>If you did not make this change, please contact support immediately.</p>
            </div>
            <p>For your security, make sure your new password is strong and unique.</p>";

        return BuildBaseTemplate(
            "Password updated",
            "Your password has been changed successfully.",
            message
        );
    }

    public string GetOwnerProfileCreatedEmail(string firstName, string businessName)
    {
        string details = BuildInfoCard(
            ("Business name:", businessName)
        );

        string message = $@"
            <p style='margin-top:0;'>Hello {firstName},</p>
            <p>Your owner profile has been created successfully.</p>
            {details}
            <p>You can now create and manage your properties on AlBooking.</p>";

        return BuildBaseTemplate(
            "Owner profile created",
            "Your owner profile has been created successfully.",
            message
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
        string details = BuildInfoCard(
            ("Property:", propertyName),
            ("City:", city),
            ("Country:", country),
            ("Type:", propertyType),
            ("Max guests:", maxGuests.ToString()),
            ("Price per night:", $"€{pricePerNight:0.##}")
        );

        string message = $@"
            <p style='margin-top:0;'>Hello {firstName},</p>
            <p>Your property has been approved successfully.</p>
            {details}
            <p>Your listing is now active and visible on the platform.</p>";

        return BuildBaseTemplate(
            "Property approved",
            "Your property has been approved successfully.",
            message
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
        string details = BuildInfoCard(
            ("Property:", propertyName),
            ("City:", city),
            ("Country:", country),
            ("Type:", propertyType),
            ("Max guests:", maxGuests.ToString()),
            ("Price per night:", $"€{pricePerNight:0.##}")
        );

        string message = $@"
            <p style='margin-top:0;'>Hello {firstName},</p>
            <p>We’re sorry to inform you that your property could not be approved.</p>
            {details}
            <p>Please review your property details and update the listing if needed.</p>";

        return BuildBaseTemplate(
            "Property declined",
            "Your property could not be approved.",
            message
        );
    }

    public string GetNewReviewEmail(string firstName, string propertyName, int rating, string comment)
    {
        string details = BuildInfoCard(
            ("Property:", propertyName),
            ("Rating:", $"{rating}/5"),
            ("Comment:", comment)
        );

        string message = $@"
            <p style='margin-top:0;'>Hello {firstName},</p>
            <p>Your property has received a new review.</p>
            {details}
            <p>You can review the details from your dashboard later.</p>";

        return BuildBaseTemplate(
            "New review received",
            "Your property has received a new review.",
            message
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
        string details = BuildInfoCard(
            ("Property:", propertyName),
            ("Check-in:", startDate.ToString("dd/MM/yyyy")),
            ("Check-out:", endDate.ToString("dd/MM/yyyy")),
            ("Guests:", guestCount.ToString()),
            ("Total price:", $"€{totalPrice:0.##}")
        );

        string message = $@"
            <p style='margin-top:0;'>Hello {firstName},</p>
            <p>You have received a new booking request for your property.</p>
            {details}
            <p>Please review this request in your dashboard.</p>";

        return BuildBaseTemplate(
            "New booking request",
            "You have received a new booking request.",
            message
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
        string details = BuildInfoCard(
            ("Property:", propertyName),
            ("City:", city),
            ("Country:", country),
            ("Type:", propertyType),
            ("Max guests:", maxGuests.ToString()),
            ("Price per night:", $"€{pricePerNight:0.##}")
        );

        string message = $@"
            <p style='margin-top:0;'>Hello {firstName},</p>
            <p>We would like to inform you that your property has been suspended.</p>
            {details}
            <p>Please review your listing or contact support for more information.</p>";

        return BuildBaseTemplate(
            "Property suspended",
            "Your property has been suspended.",
            message
        );
    }
}