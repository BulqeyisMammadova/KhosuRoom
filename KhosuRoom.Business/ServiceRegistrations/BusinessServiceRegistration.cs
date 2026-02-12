using FluentValidation;
using FluentValidation.AspNetCore;
using KhosuRoom.Business.Services.Abstractions;
using KhosuRoom.Business.Services.Implementations;
using KhosuRoom.Business.Validators.GroupValidators;
using Microsoft.Extensions.DependencyInjection;

namespace KhosuRoom.Business.ServiceRegistrations;

public static class BusinessServiceRegistration
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<GroupCreateDtoValidator>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddAutoMapper(_ => { }, typeof(BusinessServiceRegistration).Assembly);
        return services;
    }
}
