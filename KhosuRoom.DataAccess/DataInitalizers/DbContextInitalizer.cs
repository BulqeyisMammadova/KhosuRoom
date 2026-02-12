using KhosuRoom.Core.Enums;
using KhosuRoom.DataAccess.Abstractions;
using KhosuRoom.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace KhosuRoom.DataAccess.DataInitalizers;

internal class DbContextInitalizer: IContextInitalizer
{
    private readonly AppDBContext _context;
    private readonly UserManager<AppUser> _usermanager;
    private readonly RoleManager<IdentityRole<Guid>> _rolemanager;
    private readonly IConfiguration _configuration;
    private readonly string _adminUserName;
    private readonly string _adminFirstName;
    private readonly string _adminLastName;
    private readonly string _adminEmail;
    private readonly string _adminPassWord;

    public DbContextInitalizer(AppDBContext context, UserManager<AppUser> usermanager, RoleManager<IdentityRole<Guid>> rolemanager,IConfiguration configuration)
    {
        _context = context;
        _usermanager = usermanager;
        _rolemanager = rolemanager;
        _configuration = configuration;
        var section = _configuration.GetSection("AdminSettings");
        _adminUserName = section["UserName"]!;
        _adminFirstName = section["FirstName"]!;
        _adminLastName = section["LastName"]!;
        _adminEmail = section["Email"]!;
        _adminPassWord = section["Password"]!;
    }


    public async Task InitDatabaseAsync()
    {
        await _context.Database.MigrateAsync();
        await CreateRoleAsync();
        await CreateAdminAsync();

    }

    private async Task CreateAdminAsync()
    {
        AppUser adminUser = new()
        {
            UserName = _adminUserName,
            Email = _adminEmail,
            FirstName = _adminFirstName,
            LastName = _adminLastName
        };

        var result = await _usermanager.CreateAsync(adminUser, _adminPassWord);
        if (result.Succeeded)
        {
            await _usermanager.AddToRoleAsync(adminUser, GroupRole.Admin.ToString());
        }
    }

    private async Task CreateRoleAsync()
    {
        foreach (var role in Enum.GetNames(typeof(GroupRole)))
        {
            IdentityRole<Guid> appRole = new()
            {
                Name = role.ToString()
            };
            await _rolemanager.CreateAsync(appRole);
        }
    }
}
