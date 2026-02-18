using KhosuRoom.Business.Dtos.AssignmentDtos;
using KhosuRoom.Business.Dtos.DashboardDtos;
using KhosuRoom.Business.Dtos.ResultDtos;

namespace KhosuRoom.Business.Services.Abstractions;

public interface IAssignmentService
{
    Task<ResultDto> CreateAssiggn(AssignmentCreateFormDto dto);
    Task<ResultDto> UpdateAssiggn(AssignmentUpdateFormDto dto);
    Task<ResultDto> DeleteAssiggn(Guid id);
    Task<ResultDto<AssignmentGetDto>> GetByIdAssiggn(Guid id);
    Task<ResultDto<IEnumerable<AssignmentGetDto>>> GetAllAssiggn(Guid groupId);
   
}
