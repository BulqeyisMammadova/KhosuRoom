using KhosuRoom.Business.Dtos.GroupDtos;

namespace KhosuRoom.Business.Dtos;

public class GroupGetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public ICollection<GroupMemberItemDto> Members { get; set; } = [];
}
