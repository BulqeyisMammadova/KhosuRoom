using Microsoft.AspNetCore.Http;

namespace KhosuRoom.Business.Dtos.SubmissionDtos;

public class SubmissionSubmitFormDto
{
    public Guid AssignmentId { get; set; }
    public string? Comment { get; set; }
    public List<IFormFile>? Files { get; set; }
}
