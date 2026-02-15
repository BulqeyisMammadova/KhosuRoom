using AutoMapper;
using KhosuRoom.Business.Dtos.GroupMemberDtos;
using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Exceptions;
using KhosuRoom.Business.Services.Abstractions;
using KhosuRoom.Core.Enums;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using Microsoft.EntityFrameworkCore;

namespace KhosuRoom.Business.Services.Implementations;

internal class GroupMemberService : IGroupMemberService
{
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IMapper _mapper;

    public GroupMemberService(IGroupMemberRepository groupMemberRepository, IMapper mapper, IGroupRepository groupRepository)
    {
        _groupMemberRepository = groupMemberRepository;
        _mapper = mapper;
        _groupRepository = groupRepository;
    }

    public async Task<ResultDto> AddMemberAsync(Guid groupId, AddGroupMemberDto dto)
    {
        var findGroup = await _groupRepository.AnyAsync(x => x.Id == groupId);
        if (!findGroup) throw new NotFoundExceptions("Group is not found");
        var isMember = await _groupMemberRepository.AnyAsync(m => m.GroupId == groupId && m.UserId == dto.UserId);
        if (isMember) throw new AlreadyException("User already in group");
        if (!Enum.TryParse<GroupRole>(dto.Role, out var parsedRole) ||
            (parsedRole != GroupRole.Teacher && parsedRole != GroupRole.Student))
        {
            throw new BadRequestException("Role must be Teacher or Student");
        }
        var member = new GroupMember
        {
            GroupId = groupId,
            UserId = dto.UserId,
            Role = parsedRole
        };

        await _groupMemberRepository.AddAsync(member);
        await _groupMemberRepository.SaveChangesAsync();
        return new();


    }

    public async Task<ResultDto<IEnumerable<GroupMemberItemDto>>> GetMembersAsync(Guid groupId)
    {
        var findGroup = await _groupRepository.AnyAsync(x => x.Id == groupId);
        if (!findGroup) throw new NotFoundExceptions("Group is not found");
        var members = await _groupMemberRepository.GetAll()
            .Where(m => m.GroupId == groupId)
            .Include(m => m.User)
            .ToListAsync();

        var dtos = _mapper.Map<IEnumerable<GroupMemberItemDto>>(members);
        return new(dtos);

    }

    public async Task<ResultDto> JoinByCodeAsync(Guid userId, JoinGroupDto dto)
    {
        var code = dto.Code.Trim();
        var group = _groupRepository.GetAll().FirstOrDefault(g => g.Code == code);
        if (group == null) throw new NotFoundExceptions("Group with the provided code is not found");
        var isMember = await _groupMemberRepository.AnyAsync(m => m.GroupId == group.Id && m.UserId == userId);
        if (isMember) throw new AlreadyException("You are already in this group");
        var member = new GroupMember
        {
            GroupId = group.Id,
            UserId = userId,
            Role = GroupRole.Student
        };
        await _groupMemberRepository.AddAsync(member);
        await _groupMemberRepository.SaveChangesAsync();
        return new();
    }

    public async Task<ResultDto> RemoveMemberAsync(Guid groupId, Guid userId)
    {
        var findGroup = await _groupRepository.AnyAsync(x => x.Id == groupId);
        if (!findGroup) throw new NotFoundExceptions("Group is not found");
        var member = await _groupMemberRepository.GetAll()
            .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId);
        if (member == null) throw new NotFoundExceptions("Member is not found in group");
        _groupMemberRepository.Delete(member);
        await _groupMemberRepository.SaveChangesAsync();
        return new();
    }
}
