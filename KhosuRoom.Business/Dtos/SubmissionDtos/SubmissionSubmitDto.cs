using Microsoft.AspNetCore.Http;

namespace KhosuRoom.Business.Dtos.SubmissionDtos;

public class SubmissionSubmitDto
{
    public string? Text { get; set; }
    public List<IFormFile> Files { get; set; } = new();
}
