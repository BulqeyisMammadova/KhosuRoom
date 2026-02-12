using KhosuRoom.DataAccess.Abstractions;
using KhosuRoom.DataAccess.Data;
using KhosuRoom.DataAccess.DataInitalizers;
using KhosuRoom.DataAccess.Interceptors;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using KhosuRoom.DataAccess.Repository.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KhosuRoom.DataAccess.ServiceRegistartions;

public static class DataAccessRegistration
{

    public static IServiceCollection AddDataAccessServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IContextInitalizer, DbContextInitalizer>();
        services.AddDbContext<AppDBContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("Default"));
        });
        
        services.AddIdentity<AppUser, IdentityRole<Guid>>(opt =>
        {
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 6;
                 opt.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<AppDBContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<BaseAuditableInterceptor>();
        return services;
    }
}
