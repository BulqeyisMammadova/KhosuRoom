using KhosuRoom.Core.Entities.Common;
using KhosuRoom.Core.Enums;

namespace KhosuRoom.Core.Entities;

public class Notification : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    public string? RedirectUrl { get; set; }    

}
