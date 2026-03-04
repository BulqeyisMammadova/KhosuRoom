using AutoMapper;
using KhosuRoom.Business.Dtos.ChatDtos;

namespace KhosuRoom.Business.Profiles;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<SendMessageDto, ChatMessage>();

        CreateMap<ChatMessage, ChatMessageDto>()
            .ForMember(d => d.SenderName, o => o.MapFrom(s =>
                s.Sender != null
                    ? (!string.IsNullOrWhiteSpace((s.Sender.FirstName + " " + s.Sender.LastName).Trim())
                        ? (s.Sender.FirstName + " " + s.Sender.LastName).Trim()
                        : (!string.IsNullOrWhiteSpace(s.Sender.UserName)
                            ? s.Sender.UserName
                            : "User"))
                    : "User"
            ))
            .ForMember(d => d.RepliedMessageText, o => o.MapFrom(s =>
                s.ReplyToMessage == null ? null :
                (s.ReplyToMessage.Text.Length > 60
                    ? s.ReplyToMessage.Text.Substring(0, 60) + "..."
                    : s.ReplyToMessage.Text)
            ));
    }
}