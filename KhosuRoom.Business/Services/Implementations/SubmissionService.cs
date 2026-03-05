using AutoMapper;
using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Dtos.SubmissionDtos;
using KhosuRoom.Business.Exceptions;
using KhosuRoom.Business.Services.Abstractions;
using KhosuRoom.Core.Entities;
using KhosuRoom.Core.Enums;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KhosuRoom.Business.Services.Implementations;

internal class SubmissionService : ISubmissionService
{
    private readonly ISubmissionRepository _submissionRepo;
    private readonly ISubmissionAttachmentRepository _attRepo;
    private readonly IAssigmentRepository _assignmentRepo;
    private readonly IGroupMemberRepository _groupMemberRepo;
    private readonly ICloudinaryService _cloudinary;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _http;
    private readonly INotificationService _notificationService;
    private readonly UserManager<AppUser> _userManager;

    public SubmissionService(ISubmissionRepository submissionRepo, ISubmissionAttachmentRepository attRepo, IAssigmentRepository assignmentRepo, IGroupMemberRepository groupMemberRepo, ICloudinaryService cloudinary, IMapper mapper, IHttpContextAccessor http, INotificationService notificationService, UserManager<AppUser> userManager)
    {
        _submissionRepo = submissionRepo;
        _attRepo = attRepo;
        _assignmentRepo = assignmentRepo;
        _groupMemberRepo = groupMemberRepo;
        _cloudinary = cloudinary;
        _mapper = mapper;
        _http = http;
        _notificationService = notificationService;
        _userManager = userManager;
    }

    private Guid CurrentUserId()
    {
        var user = _http.HttpContext?.User ?? throw new LoginException("Unauthorized");
        var idStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(idStr)) throw new LoginException("Unauthorized");
        return Guid.Parse(idStr);
    }

    private async Task EnsureStudentAsync(Guid groupId, Guid userId)
    {
        var isStudent = await _groupMemberRepo.AnyAsync(x =>
            x.GroupId == groupId &&
            x.UserId == userId &&
            x.Role == GroupRole.Student);

        if (!isStudent)
            throw new LoginException("Only students can submit.");
    }

    private async Task EnsureTeacherAsync(Guid groupId, Guid userId)
    {
        var isTeacher = await _groupMemberRepo.AnyAsync(x =>
            x.GroupId == groupId && x.UserId == userId && x.Role == GroupRole.Teacher);
        if (!isTeacher) throw new ForbiddenException("Only teachers can access this resource.");
    }

    public async Task<ResultDto> SubmitAsync(SubmissionSubmitFormDto dto)
    {
        var studentId = CurrentUserId();

        var assignment = await _assignmentRepo.GetAll()
     .AsNoTracking()
     .FirstOrDefaultAsync(a => a.Id == dto.AssignmentId);
        if (assignment is null) throw new NotFoundExceptions("Assignment not found");

        await EnsureStudentAsync(assignment.GroupId, studentId);


        var submission = await _submissionRepo.GetAll()
            .FirstOrDefaultAsync(s => s.AssignmentId == dto.AssignmentId && s.StudentId == studentId);

        if (submission is null)
        {
            submission = new Submission
            {
                AssignmentId = dto.AssignmentId,
                StudentId = studentId
            };
            await _submissionRepo.AddAsync(submission);
            await _submissionRepo.SaveChangesAsync();
        }

        submission.Comment = dto.Comment;
        submission.SubmittedAt = DateTime.UtcNow;
        submission.Status = submission.SubmittedAt.Value > assignment.DueDate
            ? SubmissionStatus.Late
            : SubmissionStatus.Submitted;
        var isLate = submission.Status == SubmissionStatus.Late;
        var student = await _userManager.Users
    .Where(u => u.Id == studentId)
    .Select(u => new { u.UserName, u.Email })
    .FirstOrDefaultAsync();

        var studentName = student?.UserName ?? "Student";

        _submissionRepo.Update(submission);
        await _submissionRepo.SaveChangesAsync();
        var title = isLate ? "Late Submission" : "New Submission";

        var msg = isLate
            ? $"{studentName} submitted '{assignment.Title}' LATE. Due: {assignment.DueDate:yyyy-MM-dd HH:mm}"
            : $"{studentName} submitted '{assignment.Title}'.";

        var type = isLate
            ? NotificationType.SubmissionSubmittedLate
            : NotificationType.SubmissionSubmitted;

        await _notificationService.CreateForUsersAsync(
            new[] { assignment.TeacherId },                 
            title,
            msg,
            type,
            assignment.GroupId,
            $"/groups/{assignment.GroupId}/assignments/{assignment.Id}",
            senderUserId: studentId                         
        );

        if (dto.Files is not null && dto.Files.Count > 0)
        {
            var old = await _attRepo.GetAll()
                .Where(x => x.SubmissionId == submission.Id)
                .ToListAsync();

            foreach (var a in old)
            {
                await _cloudinary.RawDeleteAsync(a.FileUrl);
                _attRepo.Delete(a);
            }
            await _attRepo.SaveChangesAsync();

            foreach (var file in dto.Files)
            {
                var url = await _cloudinary.RawUploadAsync(file);

                await _attRepo.AddAsync(new SubmissionAttachment
                {
                    SubmissionId = submission.Id,
                    FileName = file.FileName,
                    FileUrl = url,
                    UploadedDate = DateTime.UtcNow
                });
            }
            await _attRepo.SaveChangesAsync();
           
            await _notificationService.CreateForUsersAsync(
                new[] { assignment.TeacherId },                 
                "New Submission",
                $"A student submitted '{assignment.Title}'.",   
                NotificationType.SubmissionSubmitted,
                assignment.GroupId,
                $"/groups/{assignment.GroupId}/assignments/{assignment.Id}",
                senderUserId: studentId                         
            );
        }

        return new ResultDto();
    }

    public async Task<ResultDto<SubmissionGetDto>> GetMySubmissionAsync(Guid assignmentId)
    {
        var studentId = CurrentUserId();

        var submission = await _submissionRepo.GetAll()
            .Include(s => s.Attachments)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);

        if (submission is null) throw new NotFoundExceptions("Submission not found");

        return new ResultDto<SubmissionGetDto>(_mapper.Map<SubmissionGetDto>(submission));
    }

   
    public async Task<ResultDto> GradeAsync(Guid submissionId, GradeSubmissionDto dto)
    {
        var teacherId = CurrentUserId();

        var submission = await _submissionRepo.GetAll()
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.Id == submissionId);

        if (submission is null) throw new NotFoundExceptions("Submission not found");
        if (submission.Assignment is null) throw new NotFoundExceptions("Assignment not found");

        await EnsureTeacherAsync(submission.Assignment.GroupId, teacherId);

        submission.Grade = dto.Grade;
        submission.Feedback = dto.Feedback;
        submission.Status = SubmissionStatus.Submitted;
        submission.GradedByTeacherId = teacherId;
        submission.GradedAt = DateTime.UtcNow;

        _submissionRepo.Update(submission);
        await _submissionRepo.SaveChangesAsync();

        await _notificationService.CreateForUsersAsync(
       new[] { submission.StudentId },
       "Grade Published",
       $"{submission.Assignment.Title}: {dto.Grade} points.",
       NotificationType.GradePublished,
       submission.Assignment.GroupId,
       $"/groups/{submission.Assignment.GroupId}/assignments/{submission.AssignmentId}",
        senderUserId: teacherId
   );


        return new ResultDto();
    }

    public async Task<ResultDto<SubmissionGetDto>> GetByIdAsync(Guid submissionId)
    {
        var teacherId = CurrentUserId();

        var submission = await _submissionRepo.GetAll()
            .Include(s => s.Attachments)
            .Include(s => s.Assignment)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == submissionId);

        if (submission is null) throw new NotFoundExceptions("Submission not found");
        if (submission.Assignment is null) throw new NotFoundExceptions("Assignment not found");

        await EnsureTeacherAsync(submission.Assignment.GroupId, teacherId);

        return new ResultDto<SubmissionGetDto>(_mapper.Map<SubmissionGetDto>(submission));
    }
}
