using KhosuRoom.Business.Dtos.GroupMemberDtos;
using KhosuRoom.Business.Dtos.ResultDtos;

namespace KhosuRoom.Business.Services.Abstractions;

public interface IGroupMemberService
{
    Task<ResultDto> AddMemberAsync(Guid groupId, AddGroupMemberDto dto);
    Task<ResultDto> JoinByCodeAsync(Guid userId, JoinGroupDto dto);
    Task<ResultDto> RemoveMemberAsync(Guid groupId, Guid userId);
    Task<ResultDto<IEnumerable<GroupMemberItemDto>>> GetMembersAsync(Guid groupId);
    
}

   
   
    