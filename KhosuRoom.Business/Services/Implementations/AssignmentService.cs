using AutoMapper;
using KhosuRoom.Business.Dtos.AssignmentDtos;
using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Exceptions;
using KhosuRoom.Business.Services.Abstractions;
using KhosuRoom.Core.Enums;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace KhosuRoom.Business.Services.Implementations;

internal class AssignmentService : IAssignmentService
{
    private readonly IAssigmentRepository _assignmentRepo;
    private readonly IAssignmentAttachmentRepository _assignmentAttachmentRepo;
    private readonly IGroupRepository _groupRepo;
    private readonly IGroupMemberRepository _groupMemberRepo;
    private readonly ICloudinaryService _cloudinary;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _http;

    public AssignmentService(IAssigmentRepository assignmentRepo, IAssignmentAttachmentRepository assignmentAttachmentRepo, IGroupRepository groupRepo, IGroupMemberRepository groupMemberRepo, ICloudinaryService cloudinary, IMapper mapper, IHttpContextAccessor http)
    {
        _assignmentRepo = assignmentRepo;
        _assignmentAttachmentRepo = assignmentAttachmentRepo;
        _groupRepo = groupRepo;
        _groupMemberRepo = groupMemberRepo;
        _cloudinary = cloudinary;
        _mapper = mapper;
        _http = http;
    }

    public async Task<ResultDto> CreateAssiggn(AssignmentCreateDto dto)
    {
        var user = _http.HttpContext?.User;
        if (user is null) throw new LoginException("Unauthorized");
        var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) throw new LoginException("Unauthorized");
        var roleId = Guid.Parse(userId);
        var isAdmin = user.IsInRole("Admin");
        if (!isAdmin)
        {
           var isTeacherInGroup = await _groupMemberRepo.AnyAsync(x=> x.GroupId == dto.GroupId && x.UserId == roleId && x.Role == GroupRole.Teacher);
           if (!isTeacherInGroup) throw new LoginException("Unauthorized");
        }

        var groupExist = await _groupRepo.AnyAsync(x => dto.GroupId == x.Id);
        if (!groupExist) throw new NotFoundExceptions("Group not found");

        var assignment = _mapper.Map<Assignment>(dto);
        assignment.TeacherId = roleId;
        await _assignmentRepo.AddAsync(assignment);
        await _assignmentRepo.SaveChangesAsync();

        if (dto.Files.Count > 0)
        {
            foreach (var file in dto.Files)
            {
                var url = await _cloudinary.FileUploadAsync(file);

                await _assignmentAttachmentRepo.AddAsync(new AssignmentAttachment
                {
                    AssignmentId = assignment.Id,
                    FileName = file.FileName,
                    FileUrl = url,
                    UploadedDate = DateTime.UtcNow
                });
            }
            await _assignmentAttachmentRepo.SaveChangesAsync();
        }
        return new();
    }

    public Task<ResultDto> DeleteAssiggn(Guid id)
    {
        

        throw new NotImplementedException();
    }

    public Task<ResultDto<IEnumerable<AssignmentGetDto>>> GetAllAssiggn(Guid groupId)
    {
        throw new NotImplementedException();
    }

    public Task<ResultDto<AssignmentGetDto>> GetByIdAssiggn(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ResultDto> UpdateAssiggn(AssignmentUpdateDto dto)
    {
        throw new NotImplementedException();
    }
}
