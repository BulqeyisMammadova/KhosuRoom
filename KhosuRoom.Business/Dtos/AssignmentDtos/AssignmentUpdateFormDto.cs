using Microsoft.AspNetCore.Http;

namespace KhosuRoom.Business.Dtos.AssignmentDtos;

public class AssignmentUpdateFormDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public List<IFormFile>? Files { get; set; }
}
