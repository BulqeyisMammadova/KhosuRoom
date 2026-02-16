using KhosuRoom.Core.Enums;

namespace KhosuRoom.Business.Dtos.DashboardDtos;

public class AssignmentDashboardDto
{
    public Guid AssignmentId { get; set; }
    public Guid GroupId { get; set; }
    public string Title { get; set; } = null!;
    public DateTime DueDate { get; set; }

    public int TotalStudents { get; set; }
    public int SubmittedCount { get; set; }
    public int LateCount { get; set; }
    public int NotSubmittedCount { get; set; }

    public List<StudentSubmissionDto> Students { get; set; } = [];
}
