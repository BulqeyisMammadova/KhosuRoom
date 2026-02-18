using KhosuRoom.Core.Entities.Common;
using KhosuRoom.Core.Enums;

namespace KhosuRoom.Core.Entities;

public class AttendanceRecord : BaseEntity
{
    public Guid AttendanceSessionId { get; set; }
    public AttendanceSession AttendanceSession { get; set; } = null!;
    public Guid StudentId { get; set; }
    public AppUser Student { get; set; } = null!;
    public AttendanceStatus Status { get; set; }
}
