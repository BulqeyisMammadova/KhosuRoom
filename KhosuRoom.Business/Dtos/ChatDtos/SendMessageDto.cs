using KhosuRoom.Core.Enums;

namespace KhosuRoom.Business.Dtos.ChatDtos;

public class SendMessageDto
{
    public Guid GroupId { get; set; }
    public string Text { get; set; } = null!;
}
