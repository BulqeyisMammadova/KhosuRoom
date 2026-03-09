using KhosuRoom.Core.Entities.Common;

namespace KhosuRoom.Core.Entities;

public class Group : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? MeetLink { get; set; }
    public string? GoogleRefreshToken { get; set; }
    public ICollection<GroupMember> Members { get; set; } = [];
    public ICollection<Assignment> Assignments { get; set; } = [];
    public ICollection<AttendanceSession> AttendanceSessions { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<ChatMessage> ChatMessages { get; set; } = [];

}
