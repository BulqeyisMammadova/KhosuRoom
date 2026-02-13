using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Dtos.UserDtos;

namespace KhosuRoom.Business.Services.Abstractions;

public interface IAuthService
{
    Task<ResultDto<AccessTokenDto>> LoginAsync(LoginDto dto);
}
