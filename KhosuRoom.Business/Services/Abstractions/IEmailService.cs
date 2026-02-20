namespace KhosuRoom.Business.Services.Abstractions;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string body);
}
