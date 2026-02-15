using KhosuRoom.Core.Entities.Common;

namespace KhosuRoom.Core.Entities;

public class Submission : BaseAutitableEntity
{
    public Guid AssignmentId { get; set; }
    public Assignment? Assignment { get; set; }
    public Guid StudentId { get; set; }
    public AppUser? Student { get; set; }
    public string? Text { get; set; }
    public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdatedAtUtc { get; set; }
    public int? Grade { get; set; }
    public string? Feedback { get; set; }
    public ICollection<SubmissionAttachment> SubmissionAttachments { get; set; } = [];


}



