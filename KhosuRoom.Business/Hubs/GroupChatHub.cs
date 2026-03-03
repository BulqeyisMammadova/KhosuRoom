using KhosuRoom.DataAccess.Repository.Abstarctions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KhosuRoom.Business.Hubs;

[Authorize]
public class GroupChatHub : Hub
{
    private readonly IGroupMemberRepository _memberRepo;

    public GroupChatHub(IGroupMemberRepository memberRepo)
    {
        _memberRepo = memberRepo;
    }

    private Guid CurrentUserId()
    {
        var idStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(idStr!);
    }

    public async Task JoinGroup(string groupId)
    {
        if (!Guid.TryParse(groupId, out var gid))
            throw new HubException("Invalid group id");

        var isMember = await _memberRepo.GetAll()
            .AnyAsync(m => m.GroupId == gid && m.UserId == CurrentUserId());

        if (!isMember)
            throw new HubException("You are not a member of this group");

        await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
    }

    public async Task LeaveGroup(string groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
    }
}