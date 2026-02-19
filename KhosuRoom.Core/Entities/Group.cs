using KhosuRoom.Core.Entities.Common;

namespace KhosuRoom.Core.Entities;

public class Group : BaseAutitableEntity
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public ICollection<GroupMember> Members { get; set; } = [];
    public ICollection<Assignment> Assignments { get; set; } = [];
    public ICollection<AttendanceSession> AttendanceSessions { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];

}
