using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Dtos.SubmissionDtos;
using KhosuRoom.Business.Services.Abstractions;

namespace KhosuRoom.Business.Services.Implementations;

internal class SubmissionService : ISubmissionService
{
    public Task<ResultDto<IEnumerable<SubmissionGetDto>>> GetAllByAssignment(Guid assignmentId)
    {
        throw new NotImplementedException();
    }

    public Task<ResultDto> Grade(Guid submissionId, GradeSubmissionDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<ResultDto> Submit(Guid assignmentId, SubmissionSubmitDto dto)
    {
        throw new NotImplementedException();
    }
}
