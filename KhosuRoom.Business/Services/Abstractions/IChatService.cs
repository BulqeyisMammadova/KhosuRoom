using KhosuRoom.Business.Dtos.ChatDtos;
using KhosuRoom.Business.Dtos.ResultDtos;

namespace KhosuRoom.Business.Services.Abstractions;

public interface IChatService
{
    Task<ChatMessageDto> SendMessage(SendMessageDto dto);
    Task<ChatMessageDto> EditMessage(EditMessageDto dto);
    Task<ChatPageDto> GetChatMessages(Guid groupId, int page = 1, int pageSize = 10);
    Task<ResultDto> DeleteMessageAsync(Guid messageId);

}
