namespace KhosuRoom.Business.Dtos.AttendanceDtos;

public class AttendanceSessionTableDto
{
    public Guid SessionId { get; set; }
    public Guid GroupId { get; set; }
    public DateOnly Date { get; set; }

    public List<AttendanceStudentRowDto> Students { get; set; } = new();
}
