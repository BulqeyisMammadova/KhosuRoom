namespace KhosuRoom.Business.Dtos.DashboardDtos;

public class StudentSubmissionDto
{
    public Guid StudentId { get; set; }
    public string FullName { get; set; } = null!;
    public string? Email { get; set; }
    public bool IsSubmitted { get; set; }
    public string? Status { get; set; } 
    public DateTime? SubmittedAt { get; set; }
    public decimal? Grade { get; set; }
}