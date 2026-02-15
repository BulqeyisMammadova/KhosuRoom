using Microsoft.AspNetCore.Http;

namespace KhosuRoom.Business.Dtos.AssignmentDtos;

public class AssignmentCreateDto
{
    public Guid GroupId { get; set; } 

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; } 

    public List<IFormFile> Files { get; set; } = new();
}
