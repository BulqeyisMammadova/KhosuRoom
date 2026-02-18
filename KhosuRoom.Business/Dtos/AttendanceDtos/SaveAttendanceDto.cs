namespace KhosuRoom.Business.Dtos.AttendanceDtos;

public class SaveAttendanceDto
{
    public Guid SessionId { get; set; }
    public List<SaveAttendanceStudentDto> Students { get; set; } = new();
}
