using KhosuRoom.Core.Entities.Common;

namespace KhosuRoom.Core.Entities;

public class AttendanceSession : BaseAutitableEntity
{
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;
    public Guid TeacherId { get; set; }
    public AppUser Teacher { get; set; } = null!;
    public DateOnly Date { get; set; }
    public ICollection<AttendanceRecord> Records { get; set; } = [];

}
