using AutoMapper;
using KhosuRoom.Business.Dtos.AssignmentDtos;
using KhosuRoom.Business.Dtos.DashboardDtos;
using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Exceptions;
using KhosuRoom.Business.Services.Abstractions;
using KhosuRoom.Core.Entities;
using KhosuRoom.Core.Enums;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KhosuRoom.Business.Services.Implementations;

internal class AssignmentService : IAssignmentService
{
    private readonly IAssigmentRepository _assignmentRepo;
    private readonly IAssignmentAttachmentRepository _attRepo;
    private readonly ISubmissionRepository _submissionRepo;
    private readonly IGroupRepository _groupRepo;
    private readonly IGroupMemberRepository _groupMemberRepo;
    private readonly ICloudinaryService _cloudinary;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _http;
    private readonly INotificationService _notificationService;

    public AssignmentService(IAssigmentRepository assignmentRepo, IAssignmentAttachmentRepository attRepo, IGroupRepository groupRepo, IGroupMemberRepository groupMemberRepo, ICloudinaryService cloudinary, IMapper mapper, IHttpContextAccessor http, ISubmissionRepository submissionRepo,INotificationService notificationService)
    {
        _assignmentRepo = assignmentRepo;
        _attRepo = attRepo;
        _groupRepo = groupRepo;
        _groupMemberRepo = groupMemberRepo;
        _cloudinary = cloudinary;
        _mapper = mapper;
        _http = http;
        _submissionRepo = submissionRepo;
        _notificationService = notificationService;
    }

    private Guid CurrentUserId()
    {
        var user = _http.HttpContext?.User ?? throw new LoginException("Unauthorized");
        var roleId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(roleId)) throw new LoginException("Unauthorized");
        return Guid.Parse(roleId);
    }

    private async Task EnsureTeacherAsync(Guid groupId)
    {
        var userId = CurrentUserId();

        var isTeacher = await _groupMemberRepo.AnyAsync(x =>
            x.GroupId == groupId &&
            x.UserId == userId &&
            x.Role == GroupRole.Teacher);

        if (!isTeacher) throw new LoginException("Unauthorized");
    }
    private async Task EnsureMemberAsync(Guid groupId)
    {
        var userId = CurrentUserId();

        var isMember = await _groupMemberRepo.AnyAsync(x =>
            x.GroupId == groupId &&
            x.UserId == userId);

        if (!isMember)
            throw new LoginException("Unauthorized");
    }

    public async Task<ResultDto> CreateAssiggn(AssignmentCreateFormDto dto)
    {
        var groupExist = await _groupRepo.AnyAsync(x => x.Id == dto.GroupId);
        if (!groupExist) throw new NotFoundExceptions("Group not found");

        await EnsureTeacherAsync(dto.GroupId);

        var assignment = _mapper.Map<Assignment>(dto);
        assignment.TeacherId = CurrentUserId();

        await _assignmentRepo.AddAsync(assignment);
        await _assignmentRepo.SaveChangesAsync();

        
        var studentIds = await _groupMemberRepo.GetAll()
            .Where(x => x.GroupId == assignment.GroupId && x.Role == GroupRole.Student)
            .Select(x => x.UserId)
            .ToListAsync();

       
        await _notificationService.CreateForUsersAsync(
    studentIds,
    "New Assignment",
    $"{assignment.Title} created new assignment",
    NotificationType.AssignmentCreated,
    assignment.GroupId,
    $"/groups/{assignment.GroupId}/assignments/{assignment.Id}",
    senderUserId: assignment.TeacherId
);

        if (dto.Files is not null && dto.Files.Count > 0)
        {
            foreach (var file in dto.Files)
            {
                var url = await _cloudinary.RawUploadAsync(file);

                await _attRepo.AddAsync(new AssignmentAttachment
                {
                    AssignmentId = assignment.Id,
                    FileName = file.FileName,
                    FileUrl = url,
                    UploadedDate = DateTime.UtcNow
                });
            }
            await _attRepo.SaveChangesAsync();
        }

        return new ResultDto();
    }


    public async Task<ResultDto> UpdateAssiggn(AssignmentUpdateFormDto dto)
    {
        var assignment = await _assignmentRepo.GetAll()
            .Include(a => a.Submissions)
            .FirstOrDefaultAsync(a => a.Id == dto.Id);

        if (assignment is null) throw new NotFoundExceptions("Assignment not found");

        await EnsureTeacherAsync(assignment.GroupId);

        _mapper.Map(dto, assignment);

        
        foreach (var s in assignment.Submissions)
        {
            if (s.SubmittedAt is null) continue;

            s.Status = s.SubmittedAt.Value <= assignment.DueDate ? SubmissionStatus.Submitted : SubmissionStatus.Late;
        }

        _assignmentRepo.Update(assignment);
        await _assignmentRepo.SaveChangesAsync();


        var studentIds = await _groupMemberRepo.GetAll()
    .Where(x => x.GroupId == assignment.GroupId && x.Role == GroupRole.Student)
    .Select(x => x.UserId)
    .ToListAsync();

        await _notificationService.CreateForUsersAsync(
      studentIds,
      "Assignment Updated",
      $"{assignment.Title} updated.",
      NotificationType.AssignmentUpdated,
      assignment.GroupId,
      $"/groups/{assignment.GroupId}/assignments/{assignment.Id}",
      senderUserId: assignment.TeacherId
  );





        if (dto.Files is not null && dto.Files.Count > 0)
        {
            var oldAtts = await _attRepo.GetAll()
                .Where(x => x.AssignmentId == assignment.Id)
                .ToListAsync();

            foreach (var old in oldAtts)
            {
                await _cloudinary.RawDeleteAsync(old.FileUrl);
                _attRepo.Delete(old);
            }
            await _attRepo.SaveChangesAsync();

            foreach (var file in dto.Files)
            {
                var url = await _cloudinary.RawUploadAsync(file);

                await _attRepo.AddAsync(new AssignmentAttachment
                {
                    AssignmentId = assignment.Id,
                    FileName = file.FileName,
                    FileUrl = url,
                    UploadedDate = DateTime.UtcNow
                });
            }
            await _attRepo.SaveChangesAsync();
        }

        return new ResultDto();
    }

    public async Task<ResultDto> DeleteAssiggn(Guid id)
    {
        var assignment = await _assignmentRepo.GetByIdAsync(id);
        if (assignment is null) throw new NotFoundExceptions("Assignment not found");

        await EnsureTeacherAsync(assignment.GroupId);

        var atts = await _attRepo.GetAll()
            .Where(x => x.AssignmentId == id)
            .ToListAsync();

        foreach (var att in atts)
        {
            await _cloudinary.RawDeleteAsync(att.FileUrl);
            _attRepo.Delete(att);
        }
        await _attRepo.SaveChangesAsync();

        _assignmentRepo.Delete(assignment);
        await _assignmentRepo.SaveChangesAsync();

        return new ResultDto();
    }

    public async Task<ResultDto<AssignmentGetDto>> GetByIdAssiggn(Guid id)
    {

        var assignment = await _assignmentRepo.GetAll()
            .Include(a => a.Attachments)
            .Include(a => a.Submissions)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
        await EnsureMemberAsync(assignment!.GroupId);


        if (assignment is null) throw new NotFoundExceptions("Assignment not found");

        return new ResultDto<AssignmentGetDto>(_mapper.Map<AssignmentGetDto>(assignment));
    }

    public async Task<ResultDto<IEnumerable<AssignmentGetDto>>> GetAllAssiggn(Guid groupId)
    {
        await EnsureMemberAsync(groupId);

        var list = await _assignmentRepo.GetAll()
            .Where(a => a.GroupId == groupId)
            .Include(a => a.Attachments)
            .Include(a => a.Submissions)
            .AsNoTracking()
            .ToListAsync();

        return new ResultDto<IEnumerable<AssignmentGetDto>>(_mapper.Map<IEnumerable<AssignmentGetDto>>(list));
    }

}


