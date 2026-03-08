using KhosuRoom.Business.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhosuRoom.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AIController : ControllerBase
{
    private readonly IAIService _aiService;

    public AIController(IAIService aiService)
    {
        _aiService = aiService;
    }

    /// <summary>
    /// Student bir tapşırıq üçün AI-dan 3 oxşar task alır.
    /// POST /api/AI/generate-tasks
    /// Body: { "title": "...", "description": "..." }
    /// </summary>
    [HttpPost("generate-tasks")]
    public async Task<IActionResult> GenerateTasks([FromBody] GenerateTasksRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { message = "Tapşırıq başlığı boş ola bilməz." });

        var tasks = await _aiService.GenerateSimilarTasksAsync(request.Title, request.Description);
        return Ok(new { tasks });
    }
}

public record GenerateTasksRequest(string Title, string? Description);
