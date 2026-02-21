using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Dtos.SubmissionDtos;

namespace KhosuRoom.Business.Services.Abstractions;

public interface ISubmissionService
{
    Task<ResultDto> SubmitAsync(SubmissionSubmitFormDto dto);
    Task<ResultDto<SubmissionGetDto>> GetMySubmissionAsync(Guid assignmentId);
    Task<ResultDto> GradeAsync(Guid submissionId, GradeSubmissionDto dto);

}


    
    