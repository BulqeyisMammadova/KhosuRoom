namespace KhosuRoom.Business.Dtos.AssignmentDtos;

public class AssignmentAttachmentItemDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public DateTime UploadedDate { get; set; }
}
