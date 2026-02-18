using KhosuRoom.Business.Dtos.DashboardDtos;
using KhosuRoom.Business.Dtos.ResultDtos;

namespace KhosuRoom.Business.Services.Abstractions;

public interface IDashboardService
{
    Task<ResultDto<AssignmentDashboardDto>> GetAssignmentDashboardAsync(Guid assignmentId);
    Task<ResultDto<StudentDashboardDto>> GetStudentDashboardAsync(Guid groupId);
}
