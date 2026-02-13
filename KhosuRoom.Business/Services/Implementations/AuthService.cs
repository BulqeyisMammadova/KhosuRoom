using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Dtos.UserDtos;
using KhosuRoom.Business.Exceptions;
using KhosuRoom.Business.Services.Abstractions;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace KhosuRoom.Business.Services.Implementations;

internal class AuthService(UserManager<AppUser> _userManager, IJWTService _jwtService) : IAuthService
{
    public async Task<ResultDto<AccessTokenDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null) throw new LoginException("Login faild");
        var isTruePassWord = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isTruePassWord) throw new LoginException("Login faild");
        AccessTokenDto tokenResult = await GetNewAccessToken(user);

        return new(tokenResult);


    }

    private async Task<AccessTokenDto> GetNewAccessToken(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        List<Claim> claims = new List<Claim>()
        {
            new Claim("FirstName",user.FirstName),
            new Claim("LastName",user.LastName),
            new Claim("UserName",user.UserName!),
            new Claim("Email",user.Email!),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("Role",roles.FirstOrDefault()?? "undefined"),

        };

        var tokenResult = _jwtService.CreateAccessToken(claims);
        user.RefreshToken = tokenResult.RefreshToken;
        user.RefreshTokenExpireDate = tokenResult.ExpiredRefreshToken;
        await _userManager.UpdateAsync(user);
        return tokenResult;
    }

    public async Task<ResultDto<AccessTokenDto>> RefreshTokenAsync(string refreshToken)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.RefreshToken == refreshToken && u.RefreshTokenExpireDate > DateTime.UtcNow);
        if (user == null) throw new LoginException("Invalid refresh token");
        var newAccessToken = await GetNewAccessToken(user);
        return new(newAccessToken);


    }
}
