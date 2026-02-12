using KhosuRoom.Business.Dtos.GroupDtos;
using KhosuRoom.Business.Dtos.ResultDtos;
namespace KhosuRoom.Business.Services.Abstractions;

public interface IGroupService
{
    Task<ResultDto> CreateGroupAsync(GroupCreateDto dto);
    Task<ResultDto<GroupGetDto>> GetGroupAsync(Guid id);
    Task<ResultDto<IEnumerable<GroupGetDto>>> GetAllGroupAsnyc();
    Task<ResultDto> UpdateGroupAsync(GroupUpdateDto dto);
    Task <ResultDto> DeleteGroupAsync(Guid id);
}
