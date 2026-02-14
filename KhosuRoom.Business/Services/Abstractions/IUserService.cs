using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Dtos.UserDtos;
using KhosuRoom.Core.Enums;

namespace KhosuRoom.Business.Services.Abstractions;

public interface IUserService
{
    //Admin
    Task<ResultDto<UserGetDto>> CreateUserAsync(AdminCreateUserDto dto, GroupRole role);
    Task<ResultDto<IEnumerable<UserGetDto>>> GetUsersAsync(string? role = null, string? q = null);
    Task<ResultDto<UserGetDto>> GetByIdAsync(Guid id);
    Task<ResultDto> UpdateAsync(Guid id, AdminUpdateUserDto dto);
    Task<ResultDto> DeactivateAsync(Guid id);

    // User
    Task<ResultDto<UserGetDto>> GetMeAsync(Guid userId);
    Task<ResultDto> UpdateMyProfileImageAsync(Guid userId, MeUpdateProfileImageDto dto);
    Task<ResultDto> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
}
