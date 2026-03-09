using KhosuRoom.Business.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace KhosuRoom.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GoogleCalendarController : ControllerBase
{
    private readonly IGoogleCalendarService _googleCalendarService;

    public GoogleCalendarController(IGoogleCalendarService googleCalendarService)
    {
        _googleCalendarService = googleCalendarService;
    }

    /// <summary>
    /// Smart endpoint: tries to create a Meet link using stored token.
    /// If no stored token exists, redirects the user to Google OAuth login.
    /// Frontend should call this instead of auth-url directly.
    /// </summary>
    [HttpGet("create-or-auth/{groupId}")]
    public async Task<IActionResult> CreateOrAuth(Guid groupId)
    {
        try
        {
            var meetLink = await _googleCalendarService.CreateMeetLinkWithStoredTokenAsync(groupId);

            if (meetLink != null)
            {
                // Success — redirect back to frontend with the link
                var successUrl = $"http://localhost:4173/dashboard?meetLink={Uri.EscapeDataString(meetLink)}&groupId={groupId}";
                return Redirect(successUrl);
            }

            // No stored token — start OAuth flow
            var authUrl = _googleCalendarService.GetAuthUrl(groupId);
            return Redirect(authUrl);
        }
        catch (Exception ex)
        {
            var errorUrl = $"http://localhost:4173/dashboard?error={Uri.EscapeDataString(ex.Message)}";
            return Redirect(errorUrl);
        }
    }

    [HttpGet("auth-url/{groupId}")]
    public IActionResult GetAuthUrl(Guid groupId)
    {
        var url = _googleCalendarService.GetAuthUrl(groupId);
        return Redirect(url);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
    {
        if (string.IsNullOrEmpty(code))
            return BadRequest("Authorization code is missing.");

        if (!Guid.TryParse(state, out Guid groupId))
            return BadRequest("Invalid group ID in state.");

        try
        {
            var meetLink = await _googleCalendarService.ExchangeCodeAndCreateMeetLinkAsync(code, groupId);
            var frontendRedirectUrl = $"http://localhost:4173/dashboard?meetLink={Uri.EscapeDataString(meetLink)}&groupId={groupId}";
            return Redirect(frontendRedirectUrl);
        }
        catch (Exception ex)
        {
            var errorRedirectUrl = $"http://localhost:4173/dashboard?error={Uri.EscapeDataString(ex.Message)}";
            return Redirect(errorRedirectUrl);
        }
    }
}
