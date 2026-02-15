namespace KhosuRoom.Business.Dtos.GroupMemberDtos;

public class AddGroupMemberDto
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = "Student";
}
