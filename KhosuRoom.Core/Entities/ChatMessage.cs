using KhosuRoom.Core.Entities.Common;
using KhosuRoom.Core.Enums;

namespace KhosuRoom.Core.Entities;

public class ChatMessage : BaseAuditableEntity
{
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public Guid SenderId { get; set; }
    public AppUser Sender { get; set; } = null!;

    public string Text { get; set; } = null!;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }

    public Guid? ReplyToMessageId { get; set; }
    public ChatMessage? ReplyToMessage { get; set; }
    public ICollection<ChatMessage> Replies { get; set; } = [];
}





    


   


