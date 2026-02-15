namespace KhosuRoom.Business.Dtos.SubmissionDtos;

public class SubmissionGetDto
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }

    public Guid StudentId { get; set; }
    public string StudentFullName { get; set; } = string.Empty;

    public string? Text { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime? LastUpdatedAtUtc { get; set; }

    public int? Grade { get; set; }
    public string? Feedback { get; set; }

    public ICollection<SubmissionAttachmentItemDto> Attachments { get; set; } = new List<SubmissionAttachmentItemDto>();
}
