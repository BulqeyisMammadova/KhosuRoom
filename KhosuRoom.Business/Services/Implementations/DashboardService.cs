using KhosuRoom.Business.Dtos.DashboardDtos;
using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Exceptions;
using KhosuRoom.Business.Services.Abstractions;
using KhosuRoom.Core.Enums;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KhosuRoom.Business.Services.Implementations;

internal class DashboardService : IDashboardService
{
    private readonly IAssigmentRepository _assignmentRepo;
    private readonly IGroupMemberRepository _groupMemberRepo;
    private readonly ISubmissionRepository _submissionRepo;
    private readonly IHttpContextAccessor _http;

    public DashboardService(IAssigmentRepository assignmentRepo, IGroupMemberRepository groupMemberRepo, ISubmissionRepository submissionRepo, IHttpContextAccessor http)
    {
        _assignmentRepo = assignmentRepo;
        _groupMemberRepo = groupMemberRepo;
        _submissionRepo = submissionRepo;
        _http = http;
    }

    private Guid CurrentUserId()
    {
        var user = _http.HttpContext?.User ?? throw new LoginException("Unauthorized");
        var idStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(idStr)) throw new LoginException("Unauthorized");
        return Guid.Parse(idStr);
    }

    private async Task EnsureTeacherAsync(Guid groupId, Guid userId)
    {
        var isTeacher = await _groupMemberRepo.AnyAsync(x =>
            x.GroupId == groupId &&
            x.UserId == userId &&
            x.Role == GroupRole.Teacher);

        if (!isTeacher) throw new LoginException("Unauthorized");
    }
    private static decimal ClampTo0_100(decimal value)
    {
        if (value < 0) return 0;
        if (value > 100) return 100;
        return value;
    }

    public async Task<ResultDto<AssignmentDashboardDto>> GetAssignmentDashboardAsync(Guid assignmentId)
    {
        var teacherId = CurrentUserId();

        var assignment = await _assignmentRepo.GetAll()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == assignmentId);

        if (assignment is null) throw new NotFoundExceptions("Assignment not found");

        await EnsureTeacherAsync(assignment.GroupId, teacherId);

        
        var students = await _groupMemberRepo.GetAll()
            .Where(gm => gm.GroupId == assignment.GroupId && gm.Role == GroupRole.Student)
            .Include(gm => gm.User)
            .AsNoTracking()
            .Select(gm => new
            {
                StudentId = gm.UserId,
                FullName = (gm.User.FirstName + " " + gm.User.LastName).Trim(),
                Email = gm.User.Email
            })
            .ToListAsync();

        
        var submissions = await _submissionRepo.GetAll()
            .Where(s => s.AssignmentId == assignmentId)
            .AsNoTracking()
            .Select(s => new
            {
                s.Id,
                s.StudentId,
                s.Status,
                s.SubmittedAt,
                s.Grade
            })
            .ToListAsync();

        var subMap = submissions
    .GroupBy(x => x.StudentId)
    .ToDictionary(
        g => g.Key,
        g => g.OrderByDescending(x => x.SubmittedAt ?? DateTime.MinValue).First()
    );

        var dto = new AssignmentDashboardDto
        {
            AssignmentId = assignment.Id,
            GroupId = assignment.GroupId,
            Title = assignment.Title,
            DueDate = assignment.DueDate,
            TotalStudents = students.Count
        };

        foreach (var st in students)
        {
            if (subMap.TryGetValue(st.StudentId, out var sub))
            {
                var isSubmitted = sub.SubmittedAt is not null &&
                                  (sub.Status == SubmissionStatus.Submitted || sub.Status == SubmissionStatus.Late);


                var progress = isSubmitted ? 100m : 0m;

                dto.Students.Add(new StudentSubmissionDto
                {
                    StudentId = st.StudentId,
                    SubmissionId = sub.Id,
                    FullName = st.FullName,
                    Email = st.Email,
                    IsSubmitted = isSubmitted,
                    Status = sub.Status,
                    SubmittedAt = sub.SubmittedAt,
                    Grade = sub.Grade,
                    ProgressPercent = progress
                });
            }
            else
            {
                dto.Students.Add(new StudentSubmissionDto
                {
                    StudentId = st.StudentId,
                    FullName = st.FullName,
                    SubmissionId = null,
                    Email = st.Email,
                    IsSubmitted = false,
                    Status = null,
                    SubmittedAt = null,
                    Grade = null,
                    ProgressPercent = 0
                });
            }
        }

        dto.SubmittedCount = dto.Students.Count(x => x.IsSubmitted && x.Status == SubmissionStatus.Submitted);
        dto.LateCount = dto.Students.Count(x => x.IsSubmitted && x.Status == SubmissionStatus.Late);
        dto.NotSubmittedCount = dto.Students.Count(x => !x.IsSubmitted);

       
        dto.Students = dto.Students
            .OrderBy(x => x.IsSubmitted) 
            .ThenByDescending(x => x.Status == SubmissionStatus.Late)
            .ThenBy(x => x.FullName)
            .ToList();

        return new (dto);
    }

    public async Task<ResultDto<StudentDashboardDto>> GetStudentDashboardAsync(Guid groupId)
    {
        var studentId = CurrentUserId();

        
        var isStudent = await _groupMemberRepo.AnyAsync(x =>
            x.GroupId == groupId &&
            x.UserId == studentId &&
            x.Role == GroupRole.Student);

        if (!isStudent) throw new LoginException("Unauthorized");
        var assignmentIds = await _assignmentRepo.GetAll()
       .Where(a => a.GroupId == groupId)
       .AsNoTracking()
       .Select(a => a.Id)
       .ToListAsync();

        var totalAssignments = assignmentIds.Count;

        if (totalAssignments == 0)
        {
            return new ResultDto<StudentDashboardDto>(new StudentDashboardDto
            {
                GroupId = groupId,
                TotalAssignments = 0,
                SubmittedCount = 0,
                LateCount = 0,
                AverageGrade = null,
                OverallProgressPercent = 0
            });
        }
        var subs = await _submissionRepo.GetAll()
        .Where(s => s.StudentId == studentId && assignmentIds.Contains(s.AssignmentId))
        .AsNoTracking()
        .Select(s => new { s.AssignmentId, s.Status, s.Grade, s.SubmittedAt })
        .ToListAsync();

        var subMap = subs
    .GroupBy(x => x.AssignmentId)
    .ToDictionary(
        g => g.Key,
        g => g.OrderByDescending(x => x.SubmittedAt ?? DateTime.MinValue).First()
    );

        int submittedCount = 0;
        int lateCount = 0;

        decimal progressSum = 0;
        decimal gradeSum = 0;
        int gradedCount = 0;
        foreach (var assId in assignmentIds)
        {
            if (subMap.TryGetValue(assId, out var sub))
            {
                var isSubmitted = sub.SubmittedAt is not null &&
                                  (sub.Status == SubmissionStatus.Submitted || sub.Status == SubmissionStatus.Late);

                if (isSubmitted)
                {
                    submittedCount++;
                    if (sub.Status == SubmissionStatus.Late) lateCount++;
                }

                if (sub.Grade.HasValue)
                {
                    var g = ClampTo0_100(sub.Grade.Value);
                    gradeSum += g;
                    gradedCount++;
                    progressSum += g; 
                }
                else
                {
                    progressSum += isSubmitted ? 100 : 0;
                }
            }
            else
            {
               
                progressSum += 0;
            }
        }

        decimal? avgGrade = gradedCount == 0 ? null : Math.Round(gradeSum / gradedCount, 2);
        var overallProgress = Math.Round(progressSum / totalAssignments, 2);

        var dto = new StudentDashboardDto
        {
            GroupId = groupId,
            TotalAssignments = totalAssignments,
            SubmittedCount = submittedCount,
            LateCount = lateCount,
            AverageGrade = avgGrade,
            OverallProgressPercent = overallProgress
        };

        return new ResultDto<StudentDashboardDto>(dto);
    }
}



