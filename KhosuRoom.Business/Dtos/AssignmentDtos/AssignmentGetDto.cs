namespace KhosuRoom.Business.Dtos.AssignmentDtos;

public class AssignmentGetDto
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid TeacherId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public IEnumerable<string> FileUrls { get; set; } = [];
    public int SubmissionCount { get; set; }
}