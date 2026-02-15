using KhosuRoom.Core.Entities.Common;

namespace KhosuRoom.Core.Entities;

public class SubmissionAttachment : BaseEntity
{
    public Guid SubmissionId { get; set; }
    public Submission? Submission { get; set; }
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
}



