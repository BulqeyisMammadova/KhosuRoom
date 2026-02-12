using KhosuRoom.DataAccess.Data;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using KhosuRoom.DataAccess.Repository.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KhosuRoom.DataAccess.ServiceRegistartions;

public static class DataAccessRegistration
{

    public static IServiceCollection AddDataAccessServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddDbContext<AppDBContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("Default"));
        });
        return services;
    }
}
