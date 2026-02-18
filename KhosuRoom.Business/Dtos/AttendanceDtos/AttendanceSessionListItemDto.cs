namespace KhosuRoom.Business.Dtos.AttendanceDtos;

public class AttendanceSessionListItemDto
{
    public Guid SessionId { get; set; }
    public DateOnly Date { get; set; }
}
