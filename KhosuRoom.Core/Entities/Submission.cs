using KhosuRoom.Core.Entities.Common;
using KhosuRoom.Core.Enums;

namespace KhosuRoom.Core.Entities;

public class Submission : BaseAuditableEntity
{
    public Guid AssignmentId { get; set; }
    public Assignment? Assignment { get; set; }

    public Guid StudentId { get; set; }
    public AppUser? Student { get; set; }

    public SubmissionStatus Status { get; set; } = SubmissionStatus.Draft;
    public string? Comment { get; set; }
    public DateTime? SubmittedAt { get; set; }

    public decimal? Grade { get; set; }
    public string? Feedback { get; set; }
    public Guid? GradedByTeacherId { get; set; }
    public AppUser? GradedByTeacher { get; set; }
    public DateTime? GradedAt { get; set; }


    public ICollection<SubmissionAttachment> Attachments { get; set; } = [];

}