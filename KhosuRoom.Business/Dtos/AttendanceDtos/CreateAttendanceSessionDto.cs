namespace KhosuRoom.Business.Dtos.AttendanceDtos;

public class CreateAttendanceSessionDto
{
    public Guid GroupId { get; set; }
    public DateOnly Date { get; set; }
}
