using KhosuRoom.Business.Dtos.GroupMemberDtos;

namespace KhosuRoom.Business.Dtos;

public class GroupGetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? MeetLink { get; set; }
    public ICollection<GroupMemberItemDto> Members { get; set; } = [];
}
