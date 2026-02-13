using KhosuRoom.Business.Dtos.TokenDtos;
using KhosuRoom.Business.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KhosuRoom.Business.Services.Implementations;

internal class JWTService : IJWTService
{
    private readonly JWTOptionsDto _options;


    public JWTService(IConfiguration configuration)
    {
        _options = configuration.GetSection("JwtSettings").Get<JWTOptionsDto>() ?? new JWTOptionsDto();
    }



    public AccessTokenDto CreateAccessToken(List<Claim> claims)
    {
        JwtHeader jwtHeader = CreateJWTHeader();
        JwtPayload payload = CreateJWTPayload(claims);

        JwtSecurityToken jwtSecurityToken = new(jwtHeader, payload);



        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        string token = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
        return new()
        {
            Token = token,
            ExpiredToken = DateTime.UtcNow.AddMinutes(_options.ExpiredDate)
        };
    }

    private JwtPayload CreateJWTPayload(List<Claim> claims)
    {
        return new(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddMinutes(_options.ExpiredDate)
                );
    }

    private JwtHeader CreateJWTHeader()
    {
        SymmetricSecurityKey symmetricSecurityKey = new(Encoding.UTF8.GetBytes(_options.SecretKey));
        SigningCredentials signingCredentials = new(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        JwtHeader jwtHeader = new(signingCredentials);
        return jwtHeader;
    }
}
