namespace KhosuRoom.Business.Dtos.AssignmentDtos;

public class AssignmentGetDto
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }

    public Guid TeacherId { get; set; }
    public string TeacherFullName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }

    public ICollection<AssignmentAttachmentItemDto> Attachments { get; set; } = [];
}
