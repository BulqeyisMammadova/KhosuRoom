using KhosuRoom.Business.Dtos.AttendanceDtos;
using KhosuRoom.Business.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhosuRoom.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _service;

    public AttendanceController(IAttendanceService service)
    {
        _service = service;
    }

    [HttpPost("sessions")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateSession([FromBody] CreateAttendanceSessionDto dto)
        => Ok(await _service.CreateSessionAsync(dto));

    [HttpGet("groups/{groupId:guid}/sessions")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetGroupSessions([FromRoute] Guid groupId)
        => Ok(await _service.GetGroupSessionsAsync(groupId));

    [HttpGet("sessions/{sessionId:guid}/table")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetTable([FromRoute] Guid sessionId)
        => Ok(await _service.GetSessionTableAsync(sessionId));

    [HttpPut("sessions/{sessionId:guid}/records")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Save([FromRoute] Guid sessionId, [FromBody] SaveAttendanceDto dto)
        => Ok(await _service.SaveAttendanceAsync(sessionId, dto));

    [HttpGet("my/{groupId:guid}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> My([FromRoute] Guid groupId)
        => Ok(await _service.GetMyAttendanceAsync(groupId));
}
