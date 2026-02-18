using KhosuRoom.Business.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhosuRoom.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DashboardsController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardsController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet("assignment/{assignmentId:guid}")]
    public async Task<IActionResult> Assignment(Guid assignmentId)
        => Ok(await _service.GetAssignmentDashboardAsync(assignmentId));
    [HttpGet("student/{groupId:guid}")]
    public async Task<IActionResult> Student(Guid groupId)
    => Ok(await _service.GetStudentDashboardAsync(groupId));
}
