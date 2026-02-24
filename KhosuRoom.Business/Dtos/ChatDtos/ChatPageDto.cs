namespace KhosuRoom.Business.Dtos.ChatDtos;

public class ChatPageDto
{
    public List<ChatMessageDto> Chats { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}









