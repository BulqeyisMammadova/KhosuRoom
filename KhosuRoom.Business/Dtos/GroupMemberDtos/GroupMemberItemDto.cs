using KhosuRoom.Core.Enums;

namespace KhosuRoom.Business.Dtos.GroupMemberDtos;

public class GroupMemberItemDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string? ProfileImageUrl { get; set; }
    public string? Role { get; set; }
}
