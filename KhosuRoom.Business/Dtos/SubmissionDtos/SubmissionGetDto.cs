using KhosuRoom.Core.Enums;

namespace KhosuRoom.Business.Dtos.SubmissionDtos;

public class SubmissionGetDto
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public string Status { get; set; } = null!;
    public string? Comment { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public decimal? Grade { get; set; }
    public string? Feedback { get; set; }
    public IEnumerable<string> FileUrls { get; set; } = [];
}