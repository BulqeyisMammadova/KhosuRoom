using KhosuRoom.Business.Dtos.EmailDtos;
using KhosuRoom.Business.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;

namespace KhosuRoom.Business.Services.Implementations;

internal class EmailService : IEmailService
{
    private readonly SmtpSettingsDto _smtpSettings;

    public EmailService(IConfiguration configuration)
    {
        _smtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettingsDto>()
            ?? new SmtpSettingsDto();
    }

    public async Task SendEmailAsync(
        string toEmail,
        string subject,
        string body,
        string? replyToEmail = null,
        string? replyToName = null)
    {
        try
        {
            var message = new MimeMessage();

           
            message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));

            
            if (!string.IsNullOrWhiteSpace(replyToEmail))
            {
                message.ReplyTo.Add(new MailboxAddress(replyToName ?? replyToEmail, replyToEmail));
            }

            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using var client = new SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, false);
            await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to send email", ex);
        }
    }
}