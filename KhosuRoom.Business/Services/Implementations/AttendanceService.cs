using AutoMapper;
using KhosuRoom.Business.Dtos.AttendanceDtos;
using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Exceptions;
using KhosuRoom.Business.Services.Abstractions;
using KhosuRoom.Core.Entities;
using KhosuRoom.Core.Enums;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using KhosuRoom.DataAccess.Repository.Abstarctions.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KhosuRoom.Business.Services.Implementations;

internal class AttendanceService : IAttendanceService
{
    private readonly IAttendanceSessionRepository _sessionRepo;
    private readonly IAttendanceRecordRepository _recordRepo;
    private readonly IGroupMemberRepository _groupMemberRepo;
    private readonly IGroupRepository _groupRepo;
    private readonly IHttpContextAccessor _http;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public AttendanceService(IAttendanceSessionRepository sessionRepo, IAttendanceRecordRepository recordRepo, IGroupMemberRepository groupMemberRepo, IGroupRepository groupRepo, IHttpContextAccessor http, IMapper mapper, INotificationService notificationService)
    {
        _sessionRepo = sessionRepo;
        _recordRepo = recordRepo;
        _groupMemberRepo = groupMemberRepo;
        _groupRepo = groupRepo;
        _http = http;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    private Guid CurrentUserId =>
        Guid.Parse(_http.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<ResultDto> CreateSessionAsync(CreateAttendanceSessionDto dto)
    {
        var exist = await _groupRepo.AnyAsync(g => g.Id == dto.GroupId);
        if (!exist) throw new NotFoundExceptions("Group not found");

        var isTeacher = await _groupMemberRepo.AnyAsync(gm =>
            gm.GroupId == dto.GroupId &&
            gm.UserId == CurrentUserId &&
            gm.Role == GroupRole.Teacher);

        if (!isTeacher)  throw new ForbiddenException("Only teacher can create attendance session");

        var exists = await _sessionRepo.AnyAsync(s => s.GroupId == dto.GroupId && s.Date == dto.Date);
        if (exists) throw new AlreadyException("Attendance session already exists for this date");

       
        var session = _mapper.Map<AttendanceSession>(dto);
        session.TeacherId = CurrentUserId; 
        await _sessionRepo.AddAsync(session);
        await _sessionRepo.SaveChangesAsync();

        
        var studentIds = await _groupMemberRepo
                        .GetAll()
                        .Where(gm => gm.GroupId == dto.GroupId && gm.Role == GroupRole.Student)
                        .Select(gm => gm.UserId)
                        .ToListAsync();

        foreach (var sid in studentIds)
        {
            await _recordRepo.AddAsync(new AttendanceRecord
            {
                AttendanceSessionId = session.Id,
                StudentId = sid,
                Status = AttendanceStatus.Absent
            });
        }

        await _recordRepo.SaveChangesAsync();
        await _notificationService.CreateForUsersAsync(
       studentIds,
       "Attendance Session Opened",
       $"Attendance session for {dto.Date:yyyy-MM-dd} has been created.",
       NotificationType.AttendanceSessionCreated,
       dto.GroupId,
       $"/groups/{dto.GroupId}/attendance",
         senderUserId: session.TeacherId

   );

        return new();
    }

    public async Task<AttendanceSessionTableDto> GetSessionTableAsync(Guid sessionId)
    {
        var session = await _sessionRepo.GetByIdAsync(sessionId);
        if (session is null)   throw new NotFoundExceptions("Session not found");

        if (session.TeacherId != CurrentUserId) throw new ForbiddenException("You can only access your own sessions");

        var records = await _recordRepo
            .GetAll()
            .Where(r => r.AttendanceSessionId == sessionId)
            .Include(r => r.Student)
            .ToListAsync();

        var studentIds = records.Select(r => r.StudentId).Distinct().ToList();

       
        var absentCounts = await _recordRepo
            .GetAll()
            .Include(r => r.AttendanceSession)
            .Where(r =>
                studentIds.Contains(r.StudentId) &&
                r.AttendanceSession.GroupId == session.GroupId &&
                r.Status == AttendanceStatus.Absent)
            .GroupBy(r => r.StudentId)
            .Select(g => new { StudentId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.StudentId, x => x.Count);

        var dto = new AttendanceSessionTableDto
        {
            SessionId = session.Id,
            GroupId = session.GroupId,
            Date = session.Date,
            Students = records.Select(r =>
            {
                var row = _mapper.Map<AttendanceStudentRowDto>(r);
                absentCounts.TryGetValue(r.StudentId, out var cnt);
                row.TotalAbsentCount = cnt;
                row.Status = r.Status.ToString();
                row.StudentId = r.StudentId;
                return row;
            })
            .OrderBy(x => x.FullName)
            .ToList()
        };

        return dto;
    }

    public async Task<ResultDto> SaveAttendanceAsync(Guid sessionId, SaveAttendanceDto dto)
    {
        var session = await _sessionRepo.GetByIdAsync(sessionId);
        if (session is null) throw new NotFoundExceptions("Session not found");

        if (session.TeacherId != CurrentUserId)    throw new ForbiddenException("You can only update your own sessions");

        var records = await _recordRepo
            .GetAll()
            .Where(r => r.AttendanceSessionId == sessionId)
            .ToListAsync();

        foreach (var item in dto.Students)
        {
            var rec = records.FirstOrDefault(r => r.StudentId == item.StudentId);
            if (rec is null)  throw new NotFoundExceptions("Student record not found in this session");

            rec.Status = item.Status;
            _recordRepo.Update(rec);

        }

        await _recordRepo.SaveChangesAsync();
        var studentIds = records.Select(r => r.StudentId).Distinct().ToList();

        await _notificationService.CreateForUsersAsync(
            studentIds,
            "Attendance Updated",
            $"Attendance for {session.Date:yyyy-MM-dd} has been updated.",
            NotificationType.AttendanceSaved,
            session.GroupId,
            $"/groups/{session.GroupId}/attendance",
            senderUserId: session.TeacherId
        );

        return new ResultDto();
    }

    public async Task<MyAttendanceDto> GetMyAttendanceAsync(Guid groupId)
    {
        var isMember = await _groupMemberRepo.AnyAsync(gm => gm.GroupId == groupId && gm.UserId == CurrentUserId);
        if (!isMember)  throw new ForbiddenException("You are not a member of this group");

        var query = _recordRepo
            .GetAll()
            .Include(r => r.AttendanceSession)
            .Where(r => r.StudentId == CurrentUserId && r.AttendanceSession.GroupId == groupId);

        var totalAbsent = await query.CountAsync(r => r.Status == AttendanceStatus.Absent);
        var totalPresent = await query.CountAsync(r => r.Status == AttendanceStatus.Present);

        var total = totalAbsent + totalPresent;
        var percent = total == 0 ? 0 : Math.Round((decimal)totalPresent * 100m / total, 2);

        return new MyAttendanceDto
        {
            GroupId = groupId,
            TotalAbsent = totalAbsent,
            TotalPresent = totalPresent,
            AttendancePercent = percent
        };
    }

    public async Task<List<AttendanceSessionListItemDto>> GetGroupSessionsAsync(Guid groupId)
    {
        if (!await _groupRepo.AnyAsync(g => g.Id == groupId)) throw new NotFoundExceptions("Group not found");

        var isTeacher = await _groupMemberRepo.AnyAsync(gm =>
            gm.GroupId == groupId &&
            gm.UserId == CurrentUserId &&
            gm.Role == GroupRole.Teacher);

        if (!isTeacher) throw new ForbiddenException("Only teacher can access sessions");

        var sessions = await _sessionRepo
            .GetAll()
            .Where(s => s.GroupId == groupId)
            .OrderByDescending(s => s.Date)
            .ToListAsync();

        var students = _mapper.Map<List<AttendanceSessionListItemDto>>(sessions);
        return students;
    }


}
