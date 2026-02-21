namespace KhosuRoom.Business.Services.Abstractions;

public interface IEmailService
{
    Task SendEmailAsync(
        string toEmail,
        string subject,
        string body,
        string? replyToEmail = null,
        string? replyToName = null);
}