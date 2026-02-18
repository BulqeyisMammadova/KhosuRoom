using KhosuRoom.Core.Enums;

namespace KhosuRoom.Business.Dtos.AttendanceDtos;

public class AttendanceStudentRowDto
{
    public Guid StudentId { get; set; }
    public string FullName { get; set; } = null!;
    public int TotalAbsentCount { get; set; }
    public AttendanceStatus Status { get; set; }
}
