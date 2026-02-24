using KhosuRoom.Core.Enums;

namespace KhosuRoom.Business.Dtos.ChatDtos;

public class ChatMessageDto 
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = null!;
    public string Text { get; set; } = null!;
    public DateTime SentAt { get; set; }
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }

    public Guid? ReplyToMessageId { get; set; }
    public string? RepliedMessageText { get; set; }
}








