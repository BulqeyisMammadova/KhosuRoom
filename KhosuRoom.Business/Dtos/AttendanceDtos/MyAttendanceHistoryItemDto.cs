namespace KhosuRoom.Business.Dtos.AttendanceDtos;

public class MyAttendanceHistoryItemDto
{
    public Guid SessionId { get; set; }
    public DateOnly Date { get; set; }
    public string Status { get; set; } = null!;
}
