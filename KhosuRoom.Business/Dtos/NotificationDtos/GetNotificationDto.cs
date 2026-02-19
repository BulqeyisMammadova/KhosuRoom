using KhosuRoom.Core.Enums;

namespace KhosuRoom.Business.Dtos.NotificationDtos;

public class GetNotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public NotificationType Type { get; set; }

    public Guid? GroupId { get; set; }
    public string? RedirectUrl { get; set; }

    public bool IsRead { get; set; }
    public DateTime CreateDate { get; set; }
}
