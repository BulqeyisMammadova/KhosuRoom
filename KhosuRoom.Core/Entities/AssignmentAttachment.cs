using KhosuRoom.Core.Entities.Common;

namespace KhosuRoom.Core.Entities;

public class AssignmentAttachment : BaseEntity
{
    public Guid AssignmentId { get; set; }
    public Assignment? Assignment { get; set; }
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
}
