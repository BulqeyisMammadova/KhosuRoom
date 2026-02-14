using FluentValidation;
using FluentValidation.AspNetCore;
using KhosuRoom.Business.Dtos.TokenDtos;
using KhosuRoom.Business.Services.Abstractions;
using KhosuRoom.Business.Services.Implementations;
using KhosuRoom.Business.Validators.GroupValidators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace KhosuRoom.Business.ServiceRegistrations;

public static class BusinessServiceRegistration
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<GroupCreateDtoValidator>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJWTService, JWTService>();
        services.AddScoped<IUserService, UserService>();

        services.AddAutoMapper(_ => { }, typeof(BusinessServiceRegistration).Assembly);

        var jwtOptionsDto = configuration.GetSection("JwtSettings").Get<JWTOptionsDto>() ?? new();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptionsDto.Issuer,
                ValidAudience = jwtOptionsDto.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptionsDto.SecretKey))
            };
        });

        services.AddAuthorization();
        return services;
    }
}
