using AutoMapper;
using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Dtos.SubmissionDtos;
using KhosuRoom.Business.Exceptions;
using KhosuRoom.Business.Services.Abstractions;
using KhosuRoom.Core.Entities;
using KhosuRoom.Core.Enums;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using Microsoft.AspNetCore.Http;
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

    public SubmissionService(ISubmissionRepository submissionRepo, ISubmissionAttachmentRepository attRepo, IAssigmentRepository assignmentRepo, IGroupMemberRepository groupMemberRepo, ICloudinaryService cloudinary, IMapper mapper, IHttpContextAccessor http)
    {
        _submissionRepo = submissionRepo;
        _attRepo = attRepo;
        _assignmentRepo = assignmentRepo;
        _groupMemberRepo = groupMemberRepo;
        _cloudinary = cloudinary;
        _mapper = mapper;
        _http = http;
    }

    private Guid CurrentUserId()
    {
        var user = _http.HttpContext?.User ?? throw new LoginException("Unauthorized");
        var idStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(idStr)) throw new LoginException("Unauthorized");
        return Guid.Parse(idStr);
    }

    private async Task EnsureMemberAsync(Guid groupId, Guid userId)
    {
        var isMember = await _groupMemberRepo.AnyAsync(x => x.GroupId == groupId && x.UserId == userId);
        if (!isMember) throw new LoginException("Unauthorized");
    }

    private async Task EnsureTeacherAsync(Guid groupId, Guid userId)
    {
        var isTeacher = await _groupMemberRepo.AnyAsync(x =>
            x.GroupId == groupId && x.UserId == userId && x.Role == GroupRole.Teacher);
        if (!isTeacher) throw new LoginException("Unauthorized");
    }

    public async Task<ResultDto> SubmitAsync(SubmissionSubmitFormDto dto)
    {
        var studentId = CurrentUserId();

        var assignment = await _assignmentRepo.GetByIdAsync(dto.AssignmentId);
        if (assignment is null) throw new NotFoundExceptions("Assignment not found");

        await EnsureMemberAsync(assignment.GroupId, studentId);

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

        _submissionRepo.Update(submission);
        await _submissionRepo.SaveChangesAsync();

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

    public async Task<ResultDto<IEnumerable<SubmissionGetDto>>> GetSubmissionsByAssignmentAsync(Guid assignmentId)
    {
        var teacherId = CurrentUserId();

        var assignment = await _assignmentRepo.GetByIdAsync(assignmentId);
        if (assignment is null) throw new NotFoundExceptions("Assignment not found");

        await EnsureTeacherAsync(assignment.GroupId, teacherId);

        var list = await _submissionRepo.GetAll()
            .Where(s => s.AssignmentId == assignmentId)
            .Include(s => s.Attachments)
            .AsNoTracking()
            .ToListAsync();

        return new ResultDto<IEnumerable<SubmissionGetDto>>(_mapper.Map<IEnumerable<SubmissionGetDto>>(list));
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
        submission.GradedByTeacherId = teacherId;
        submission.GradedAt = DateTime.UtcNow;

        _submissionRepo.Update(submission);
        await _submissionRepo.SaveChangesAsync();

        return new ResultDto();
    }
}
