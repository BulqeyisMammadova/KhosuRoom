using AutoMapper;
using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Dtos.UserDtos;
using KhosuRoom.Business.Exceptions;
using KhosuRoom.Business.Services.Abstractions;
using KhosuRoom.Core.Entities;
using KhosuRoom.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KhosuRoom.Business.Services.Implementations;

internal class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public UserService(UserManager<AppUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    private static string Normalize(string s)
        => s.Trim().ToLower().Replace(" ", "");

    private async Task<string> GenerateUniqueUserNameAsync(string firstName, string lastName, Guid? ignoreId = null)
    {
        var baseName = $"{Normalize(firstName)}.{Normalize(lastName)}";
        var userName = baseName;
        int counter = 1;

        while (await _userManager.Users.AnyAsync(x =>
            (ignoreId == null || x.Id != ignoreId) &&
            x.UserName == userName))
        {
            userName = $"{baseName}{counter}";
            counter++;
        }

        return userName;
    }

    private async Task<string> GenerateUniqueEmailAsync(string firstName, string lastName, Guid? ignoreId = null)
    {
        var baseEmail = $"{Normalize(firstName)}.{Normalize(lastName)}";
        const string domain = "@gmail.com";

        var email = baseEmail + domain;
        int counter = 1;

        while (await _userManager.Users.AnyAsync(x =>
            (ignoreId == null || x.Id != ignoreId) &&
            x.Email == email))
        {
            email = $"{baseEmail}{counter}{domain}";
            counter++;
        }

        return email;
    }

    
    public async Task<ResultDto<UserGetDto>> CreateUserAsync(AdminCreateUserDto dto, GroupRole role)
    {
        

        var user = _mapper.Map<AppUser>(dto);

        user.UserName = await GenerateUniqueUserNameAsync(dto.FirstName, dto.LastName);
        user.Email = await GenerateUniqueEmailAsync(dto.FirstName, dto.LastName);

        user.IsActive = true;
        user.MustChangePassword = true;
        const string DefaultTempPassword = "Temp123!";
        var create = await _userManager.CreateAsync(user, DefaultTempPassword);

        if (!create.Succeeded)
            throw new BadRequestException(string.Join(" | ", create.Errors.Select(e => e.Description)));

        var addRole = await _userManager.AddToRoleAsync(user, role.ToString());
        if (!addRole.Succeeded)
            throw new BadRequestException(string.Join(" | ", addRole.Errors.Select(e => e.Description)));

        var resultDto = _mapper.Map<UserGetDto>(user);
        resultDto.Roles = await _userManager.GetRolesAsync(user);

        return new(resultDto);
    }

    public async Task<ResultDto<IEnumerable<UserGetDto>>> GetUsersAsync(string? role = null, string? q = null)
    {
        var query = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim().ToLower();

            query = query.Where(u =>
                (u.FirstName ?? "").ToLower().Contains(q) ||
                (u.LastName ?? "").ToLower().Contains(q) ||
                (u.Email ?? "").ToLower().Contains(q) ||
                (u.UserName ?? "").ToLower().Contains(q));
        }

        var users = await query.ToListAsync();

        if (!string.IsNullOrWhiteSpace(role))
        {
            role = role.Trim();
            var filtered = new List<AppUser>();

            foreach (var u in users)
                if (await _userManager.IsInRoleAsync(u, role))
                    filtered.Add(u);

            users = filtered;
        }

        var dtos = new List<UserGetDto>();
        foreach (var u in users)
        {
            var dto = _mapper.Map<UserGetDto>(u);
            dto.Roles = await _userManager.GetRolesAsync(u);
            dtos.Add(dto);
        }

        return new(dtos);
    }

    public async Task<ResultDto<UserGetDto>> GetByIdAsync(Guid id)
    {
        var user = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (user is null) throw new NotFoundExceptions("User not found");

        var dto = _mapper.Map<UserGetDto>(user);
        dto.Roles = await _userManager.GetRolesAsync(user);
        return new(dto);
    }

    
    public async Task<ResultDto> UpdateAsync(Guid id, AdminUpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) throw new NotFoundExceptions("User not found");

        var oldFirst = user.FirstName;
        var oldLast = user.LastName;

       
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.IsActive = dto.IsActive;

        var nameChanged =
            !string.Equals(oldFirst, user.FirstName, StringComparison.Ordinal) ||
            !string.Equals(oldLast, user.LastName, StringComparison.Ordinal);

        if (nameChanged)
        {
            user.UserName = await GenerateUniqueUserNameAsync(user.FirstName, user.LastName, user.Id);
            user.Email = await GenerateUniqueEmailAsync(user.FirstName, user.LastName, user.Id);
        }

        var update = await _userManager.UpdateAsync(user);
        if (!update.Succeeded)
            throw new BadRequestException(string.Join(" | ", update.Errors.Select(e => e.Description)));

        return new();
    }

    public async Task<ResultDto> DeactivateAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) throw new NotFoundExceptions("User not found");

        user.IsActive = false;

        var update = await _userManager.UpdateAsync(user);
        if (!update.Succeeded)
            throw new BadRequestException(string.Join(" | ", update.Errors.Select(e => e.Description)));

        return new();
    }

    public async Task<ResultDto<UserGetDto>> GetMeAsync(Guid userId)
        => await GetByIdAsync(userId);

    public async Task<ResultDto> UpdateMyProfileImageAsync(Guid userId, MeUpdateProfileImageDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) throw new NotFoundExceptions("User not found");

        user.ProfileImageUrl = dto.ProfileImageUrl;

        var update = await _userManager.UpdateAsync(user);
        if (!update.Succeeded)
            throw new BadRequestException(string.Join(" | ", update.Errors.Select(e => e.Description)));

        return new();
    }

    public async Task<ResultDto> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) throw new NotFoundExceptions("User not found");

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(" | ", result.Errors.Select(e => e.Description)));

        user.MustChangePassword = false;
        await _userManager.UpdateAsync(user);

        return new();
    }
}
