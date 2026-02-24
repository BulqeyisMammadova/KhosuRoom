namespace KhosuRoom.Business.Dtos.ChatDtos;

public class EditMessageDto
{
    public Guid MessageId { get; set; }
    public string Text { get; set; } = null!;
}
