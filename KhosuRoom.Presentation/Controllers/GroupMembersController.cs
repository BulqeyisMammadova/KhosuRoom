using KhosuRoom.Business.Dtos.GroupMemberDtos;
using KhosuRoom.Business.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KhosuRoom.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GroupMembersController : ControllerBase
{
    private readonly IGroupMemberService _groupMemberService;
    public GroupMembersController(IGroupMemberService groupMemberService)
    {
        _groupMemberService = groupMemberService;
    }


    [Authorize]
    [HttpGet("{groupId:guid}/members")]
    public async Task<IActionResult> GetMembers(Guid groupId)
    {
        var result = await _groupMemberService.GetMembersAsync(groupId);
        return Ok(result);
    }

    
    [Authorize(Roles = "Admin")]
    [HttpPost("{groupId:guid}/members")]
    public async Task<IActionResult> AddMember(Guid groupId, [FromBody] AddGroupMemberDto dto)
    {
        var result = await _groupMemberService.AddMemberAsync(groupId, dto);
        return Ok(result);
    }


    [Authorize(Roles = "Admin")]
    [HttpDelete("{groupId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid groupId, Guid userId)
    {
        var result = await _groupMemberService.RemoveMemberAsync(groupId, userId);
        return Ok(result);
    }



    [Authorize]
    [HttpPost("join")]
    public async Task<IActionResult> JoinByCode([FromBody] JoinGroupDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdStr))
            return Unauthorized();

        var userId = Guid.Parse(userIdStr);

        var result = await _groupMemberService.JoinByCodeAsync(userId, dto);
        return Ok(result);
    }









}
