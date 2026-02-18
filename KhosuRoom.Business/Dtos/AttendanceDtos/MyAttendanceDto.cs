namespace KhosuRoom.Business.Dtos.AttendanceDtos;

public class MyAttendanceDto
{
    public Guid GroupId { get; set; }
    public int TotalAbsent { get; set; }
    public int TotalPresent { get; set; }
    public decimal AttendancePercent { get; set; } 
}