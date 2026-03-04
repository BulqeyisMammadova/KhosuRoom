using AutoMapper;
using KhosuRoom.Business.Dtos.ChatDtos;
using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Exceptions;
using KhosuRoom.Business.Hubs;
using KhosuRoom.Business.Services.Abstractions;
using KhosuRoom.Core.Enums;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace KhosuRoom.Business.Services.Implementations;

internal class ChatService : IChatService
{
    private readonly IHttpContextAccessor _http;
    private readonly IChatMessageRepository _chatMessageRepository;
    private readonly IGroupMemberRepository _memberRepo;
    private readonly IMapper _mapper;
    private readonly IHubContext<GroupChatHub> _hub;
    
    public ChatService(IHttpContextAccessor http, IChatMessageRepository chatMessageRepository, IGroupMemberRepository memberRepo, IMapper mapper, IHubContext<GroupChatHub> hub)
    {
        _http = http;
        _chatMessageRepository = chatMessageRepository;
        _memberRepo = memberRepo;
        _mapper = mapper;
        _hub = hub;
    }

    private Guid CurrentUser()
    {
        var userId = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsed))
            throw new LoginException("User not authenticated");

        return parsed;
    }
   

    private async Task<GroupMember> GetMember(Guid groupId)
    {
        var member = await _memberRepo.GetAll().FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == CurrentUser());
        if (member == null) throw new NotFoundExceptions("You are not a member of this group");
        return member;
    }


    public async Task<ChatMessageDto> SendMessage(SendMessageDto dto)
    {
         await GetMember(dto.GroupId);
       

       

        var message = _mapper.Map<ChatMessage>(dto);
        message.SenderId = CurrentUser();
        message.SentAt = DateTime.UtcNow;

        await _chatMessageRepository.AddAsync(message);
        await _chatMessageRepository.SaveChangesAsync();

        var dbMsg = await _chatMessageRepository.GetAll()
            .Include(x => x.Sender)
            .Include(x => x.ReplyToMessage)
            .FirstAsync(x => x.Id == message.Id);

        var result = _mapper.Map<ChatMessageDto>(dbMsg);


        await _hub.Clients.Group(dto.GroupId.ToString())
            .SendAsync("ReceiveMessage", result);

        return result;
    }

    public async Task<ChatMessageDto> EditMessage(EditMessageDto dto)
    {
        var message = await _chatMessageRepository.GetAll()
            .Include(x => x.Sender)
            .Include(x => x.ReplyToMessage)
            .FirstOrDefaultAsync(x => x.Id == dto.MessageId);
        if (message is null) throw new NotFoundExceptions("Message not found");

        await GetMember(message.GroupId);


        if (message.SenderId != CurrentUser())
            throw new ForbiddenException("You can only edit your own message");

        message.Text = dto.Text;
        message.IsEdited = true;
        message.EditedAt = DateTime.UtcNow;

        _chatMessageRepository.Update(message);
        await _chatMessageRepository.SaveChangesAsync();


        var result = _mapper.Map<ChatMessageDto>(message);
        await _hub.Clients.Group(message.GroupId.ToString())
            .SendAsync("MessageEdited", result);
        return result;
    }

    public async Task<ResultDto> DeleteMessageAsync(Guid messageId)
    {
        var message = await _chatMessageRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.Id == messageId);

        if (message is null)
            throw new NotFoundExceptions("Message not found");

        await GetMember(message.GroupId);

        if (message.SenderId != CurrentUser())
            throw new ForbiddenException("You can only delete your own message");

        _chatMessageRepository.Delete(message);
        await _chatMessageRepository.SaveChangesAsync();

        await _hub.Clients.Group(message.GroupId.ToString())
            .SendAsync("MessageDeleted", messageId);

        return new ResultDto();
    }



    public async Task<ChatPageDto> GetChatMessages(Guid groupId, int page = 1, int pageSize = 10)
    {
        await GetMember(groupId);
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var messagesQuery = _chatMessageRepository.GetAll()
            .Where(x => x.GroupId == groupId)
            .Include(x => x.Sender)
            .Include(x => x.ReplyToMessage)
            .OrderByDescending(x => x.SentAt);


        var total = await messagesQuery.CountAsync();
        var items = await messagesQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new ChatPageDto
        {
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
            Chats = _mapper.Map<List<ChatMessageDto>>(items)
        };

    }

}








