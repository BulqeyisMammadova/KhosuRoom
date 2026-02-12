using KhosuRoom.Core.Entities.Common;
using KhosuRoom.Core.Enums;

namespace KhosuRoom.Core.Entities;

public class GroupMember : BaseEntity
{
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public GroupRole Role { get; set; }
}
