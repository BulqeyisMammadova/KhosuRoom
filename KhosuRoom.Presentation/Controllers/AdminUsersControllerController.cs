using KhosuRoom.Business.Dtos.UserDtos;
using KhosuRoom.Business.Services.Abstractions;
using KhosuRoom.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhosuRoom.Presentation.Controllers;

[Route("api/admin/users")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly IUserService _userService;
    public AdminUsersController(IUserService userService) => _userService = userService;

    [HttpPost("teachers")]
    public async Task<IActionResult> CreateTeacher([FromBody] AdminCreateUserDto dto)
    {
        var result = await _userService.CreateUserAsync(dto, GroupRole.Teacher);
        return Ok(result);
    }
    
    [HttpPost("students")]
    public async Task<IActionResult> CreateStudent([FromBody] AdminCreateUserDto dto)
    {
        var result = await _userService.CreateUserAsync(dto, GroupRole.Student);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] string? role, [FromQuery] string? q)
    {
        var result = await _userService.GetUsersAsync(role, q);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AdminUpdateUserDto dto)
    {
        var result = await _userService.UpdateAsync(id, dto);
        return Ok(result);
    }
    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await _userService.DeactivateAsync(id);
        return Ok(result);
    }
}
