using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Dtos.SubmissionDtos;

namespace KhosuRoom.Business.Services.Abstractions;

public interface ISubmissionService
{
    Task<ResultDto> Submit(Guid assignmentId, SubmissionSubmitDto dto);
    Task<ResultDto<IEnumerable<SubmissionGetDto>>> GetAllByAssignment(Guid assignmentId);
    Task<ResultDto> Grade(Guid submissionId, GradeSubmissionDto dto);

}
