using AutoMapper;
using KhosuRoom.Business.Dtos.GroupMemberDtos;
using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Exceptions;
using KhosuRoom.Business.Services.Abstractions;
using KhosuRoom.Core.Enums;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KhosuRoom.Business.Services.Implementations;

internal class GroupMemberService : IGroupMemberService
{
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IAttendanceSessionRepository _attendanceSessionRepository;
    private readonly IAttendanceRecordRepository _attendanceRecordRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public GroupMemberService(IGroupMemberRepository groupMemberRepository, IGroupRepository groupRepository, IAttendanceSessionRepository attendanceSessionRepository, IAttendanceRecordRepository attendanceRecordRepository, IMapper mapper, UserManager<AppUser> userManager)
    {
        _groupMemberRepository = groupMemberRepository;
        _groupRepository = groupRepository;
        _attendanceSessionRepository = attendanceSessionRepository;
        _attendanceRecordRepository = attendanceRecordRepository;
        _mapper = mapper;
        _userManager = userManager;
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

        if (parsedRole == GroupRole.Student)
        {
            await SeedAttendanceRecordsForStudentAsync(groupId, dto.UserId);
        }
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
        if (string.IsNullOrWhiteSpace(code)) throw new BadRequestException("Code is required");

        var group = await _groupRepository.GetAll().FirstOrDefaultAsync(g => g.Code == code);
        if (group == null) throw new NotFoundExceptions("Group with the provided code is not found");

        var isMember = await _groupMemberRepository.AnyAsync(m => m.GroupId == group.Id && m.UserId == userId);
        if (isMember) throw new AlreadyException("You are already in this group");

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) throw new NotFoundExceptions("User not found");

        var roles = await _userManager.GetRolesAsync(user);
        var resolvedRole = roles.Contains("Teacher")
            ? GroupRole.Teacher
            : roles.Contains("Student")
                ? GroupRole.Student
                : throw new BadRequestException("Only Teacher or Student users can join with group code");

        var member = new GroupMember
        {
            GroupId = group.Id,
            UserId = userId,
            Role = resolvedRole
        };

        await _groupMemberRepository.AddAsync(member);
        await _groupMemberRepository.SaveChangesAsync();

        if (resolvedRole == GroupRole.Student)
        {
            await SeedAttendanceRecordsForStudentAsync(group.Id, userId);
        }

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

    private async Task SeedAttendanceRecordsForStudentAsync(Guid groupId, Guid studentId)
    {
        var sessionIds = await _attendanceSessionRepository.GetAll()
            .Where(s => s.GroupId == groupId)
            .Select(s => s.Id)
            .ToListAsync();

        if (sessionIds.Count == 0) return;

        var existingSessionIds = await _attendanceRecordRepository.GetAll()
            .Where(r => r.StudentId == studentId && sessionIds.Contains(r.AttendanceSessionId))
            .Select(r => r.AttendanceSessionId)
            .ToListAsync();

        var missingSessionIds = sessionIds.Except(existingSessionIds).ToList();
        if (missingSessionIds.Count == 0) return;

        foreach (var sessionId in missingSessionIds)
        {
            await _attendanceRecordRepository.AddAsync(new AttendanceRecord
            {
                AttendanceSessionId = sessionId,
                StudentId = studentId,
                Status = AttendanceStatus.Absent
            });
        }

        await _attendanceRecordRepository.SaveChangesAsync();
    }
}
