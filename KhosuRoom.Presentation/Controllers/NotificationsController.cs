using KhosuRoom.Business.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KhosuRoom.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("my")]
    public async Task<IActionResult> My([FromQuery] int take = 50)
        => Ok(await _notificationService.GetMyNotificationsAsync(take));

    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount()
        => Ok(await _notificationService.GetUnreadCountAsync());

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        var result = await _notificationService.MarkAsReadAsync(id);
        return Ok(result);
    }
       

}
