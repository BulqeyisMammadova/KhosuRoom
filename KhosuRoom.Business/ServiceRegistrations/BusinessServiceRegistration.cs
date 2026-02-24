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
        AddScope(services);

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

    private static void AddScope(IServiceCollection services)
    {
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJWTService, JWTService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGroupMemberService, GroupMemberService>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<ISubmissionService, SubmissionService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IChatService, ChatService>();
    }
}
