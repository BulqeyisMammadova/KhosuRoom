namespace KhosuRoom.Business.Services.Abstractions;

public interface IEmailTemplateService
{
    /// <summary>
    /// Builds a responsive HTML email for notifications.
    /// Encodes title/message to avoid HTML injection.
    /// </summary>
    string BuildNotificationEmail(string title, string message, string? senderName = null, string? redirectUrl = null);
}