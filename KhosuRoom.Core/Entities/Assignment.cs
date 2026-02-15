using KhosuRoom.Core.Entities.Common;

namespace KhosuRoom.Core.Entities;

public class Assignment : BaseAutitableEntity
{
    public Guid GroupId { get; set; }
    public Group? Group { get; set; }
    public Guid TeacherId { get; set; }
    public AppUser? Teacher { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; } 
    public DateTime DueDate { get; set; }
    public ICollection<AssignmentAttachment> Attachments { get; set; } = [];
    public ICollection<Submission> Submissions { get; set; } = [];

}
