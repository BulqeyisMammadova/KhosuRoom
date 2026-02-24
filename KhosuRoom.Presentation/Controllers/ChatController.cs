using KhosuRoom.Business.Dtos.ChatDtos;
using KhosuRoom.Business.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KhosuRoom.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }


    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
    {
        var result = await _chatService.SendMessage(dto);
        return Ok(result);
    }

    [HttpPut("edit")]
    public async Task<IActionResult> EditMessage([FromBody] EditMessageDto dto)
    {
        var result = await _chatService.EditMessage(dto);
        return Ok(result);
    }

    [HttpGet("groups/{groupId:guid}")]
    public async Task<IActionResult> GetChatMessages(Guid groupId, int page = 1, int pageSize = 10)
    {
        var result = await _chatService.GetChatMessages(groupId, page, pageSize);
        return Ok(result);
    }

    [HttpDelete("{messageId:guid}")]
    public async Task<IActionResult> DeleteMessage(Guid messageId)
    {
      var result =  await _chatService.DeleteMessageAsync(messageId);
        return Ok(result);
    }



}
