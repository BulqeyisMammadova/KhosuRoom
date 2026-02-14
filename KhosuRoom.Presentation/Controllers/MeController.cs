using KhosuRoom.Business.Dtos.UserDtos;
using KhosuRoom.Business.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KhosuRoom.Presentation.Controllers;

[Route("api/me")]
[ApiController]
[Authorize]
public class MeController : ControllerBase
{
    private readonly IUserService _userService;

    public MeController(IUserService userService)
    {
        _userService = userService;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet]
    public async Task<IActionResult> GetMe()
    {
        var result = await _userService.GetMeAsync(GetUserId());
        return Ok(result);
    }

    [HttpPatch("profile-image")]
    public async Task<IActionResult> UpdateProfileImage(MeUpdateProfileImageDto dto)
    {
        var result = await _userService.UpdateMyProfileImageAsync(GetUserId(), dto);
        return Ok(result);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var result = await _userService.ChangePasswordAsync(GetUserId(), dto);
        return Ok(result);
    }
}
