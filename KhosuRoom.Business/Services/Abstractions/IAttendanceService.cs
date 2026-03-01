using KhosuRoom.Business.Dtos.AttendanceDtos;
using KhosuRoom.Business.Dtos.ResultDtos;

namespace KhosuRoom.Business.Services.Abstractions;

public interface IAttendanceService
{
    Task<ResultDto> CreateSessionAsync(CreateAttendanceSessionDto dto);

    Task<List<AttendanceSessionListItemDto>> GetGroupSessionsAsync(Guid groupId);

    Task<AttendanceSessionTableDto> GetSessionTableAsync(Guid sessionId);

    Task<ResultDto> SaveAttendanceAsync(Guid sessionId, SaveAttendanceDto dto);

    Task<MyAttendanceDto> GetMyAttendanceAsync(Guid groupId);
    Task<List<MyAttendanceHistoryItemDto>> GetMyAttendanceHistoryAsync(Guid groupId);
}
