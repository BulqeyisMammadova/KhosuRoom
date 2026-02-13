using System.Security.Claims;

namespace KhosuRoom.Business.Services.Abstractions;

public interface IJWTService
{
    AccessTokenDto CreateAccessToken(List<Claim> claims);
}
