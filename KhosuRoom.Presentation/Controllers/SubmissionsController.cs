using KhosuRoom.Business.Dtos.SubmissionDtos;
using KhosuRoom.Business.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhosuRoom.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _service;
    public SubmissionsController(ISubmissionService service) => _service = service;

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromForm] SubmissionSubmitFormDto dto)
    {
        var result = await _service.SubmitAsync(dto);
        return Ok(result);
    }

    [HttpGet("my/{assignmentId:guid}")]
    public async Task<IActionResult> My(Guid assignmentId)
    {
        var result = await _service.GetMySubmissionAsync(assignmentId);
        return Ok(result);
    }

    [HttpGet("assignment/{assignmentId:guid}")]
    public async Task<IActionResult> ByAssignment(Guid assignmentId)
    {
        var result = await _service.GetSubmissionsByAssignmentAsync(assignmentId);
        return Ok(result);
    }

    [HttpPost("{submissionId:guid}/grade")]
    public async Task<IActionResult> Grade(Guid submissionId, [FromBody] GradeSubmissionDto dto)
    {
        var result = await _service.GradeAsync(submissionId, dto);
        return Ok(result);
    }
}
