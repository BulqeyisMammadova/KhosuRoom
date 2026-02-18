namespace KhosuRoom.Business.Dtos.DashboardDtos;

public class StudentDashboardDto
{
    public Guid GroupId { get; set; }

    public int TotalAssignments { get; set; }
    public int SubmittedCount { get; set; }
    public int LateCount { get; set; }

    public decimal? AverageGrade { get; set; } 
    public decimal OverallProgressPercent { get; set; } 
}
