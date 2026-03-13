using BookingPlatform.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Infrastructure.Services;

public class SendGridEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public SendGridEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string message)
    {
        var apiKey = _config["SendGrid:ApiKey"]
            ?? throw new Exception("SendGrid API key missing");
        var client = new SendGridClient(apiKey);

        var from = new EmailAddress(
            _config["SendGrid:FromEmail"],
            _config["SendGrid:FromName"]
        );

        var toEmail = new EmailAddress(to);

        var msg = MailHelper.CreateSingleEmail(
            from,
            toEmail,
            subject,
            message,
            message
        );

        await client.SendEmailAsync(msg);
    }
}