using KhosuRoom.Core.Enums;

namespace KhosuRoom.Business.Dtos.AttendanceDtos;

public class SaveAttendanceStudentDto
{
    public Guid StudentId { get; set; }
    public AttendanceStatus Status { get; set; } 
}
