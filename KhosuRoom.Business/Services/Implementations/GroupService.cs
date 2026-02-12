using AutoMapper;
using KhosuRoom.Business.Dtos.GroupDtos;
using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Exceptions;
using KhosuRoom.Business.Services.Abstractions;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using Microsoft.EntityFrameworkCore;

namespace KhosuRoom.Business.Services.Implementations;

internal class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IMapper _mapper;

    public GroupService(IGroupRepository groupRepository, IMapper mapper)
    {
        _groupRepository = groupRepository;
        _mapper = mapper;
    }

    public async Task<ResultDto> CreateGroupAsync(GroupCreateDto dto)
    {
        var isExistGroup = await _groupRepository.AnyAsync(g => g.Name.Trim().ToLower() == dto.Name.Trim().ToLower());
        if(isExistGroup)
        {
            throw new AlreadyException("Group with the same name already exists");
        }
        var group = _mapper.Map<Group>(dto);
       group.Code = await GenerateUniqueCodeAsync();
       await _groupRepository.AddAsync(group);
       await _groupRepository.SaveChangesAsync();
        return new();


    }

    public async Task <ResultDto> DeleteGroupAsync(Guid id)
    {
        var group =await _groupRepository.GetByIdAsync(id);
        if (group is null)
        {
            throw new NotFoundExceptions("Group not found");
        }
        _groupRepository.Delete(group);
        await _groupRepository.SaveChangesAsync();
        return new();
    }

    public async Task<ResultDto<IEnumerable<GroupGetDto>>> GetAllGroupAsnyc()
    {
        var groups = await _groupRepository.GetAll().Include(g => g.Members).ThenInclude(m => m.User).AsNoTracking() .ToListAsync();
        var dtos = _mapper.Map<IEnumerable<GroupGetDto>>(groups);
        return new(dtos);
    }

    public async Task<ResultDto<GroupGetDto>> GetGroupAsync(Guid id)
    {
        var group = await _groupRepository.GetAll()
            .Include(g => g.Members)
                .ThenInclude(m => m.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group is null)
            throw new NotFoundExceptions("Group not found");

        var dto =  _mapper.Map<GroupGetDto>(group);
        return new(dto);
    }

    public async Task<ResultDto> UpdateGroupAsync(GroupUpdateDto dto)
    {
        var group = await _groupRepository.GetByIdAsync(dto.Id);
        if (group is null)
        {
            throw new NotFoundExceptions("Group not found");
        }
        var isExistGroup = await _groupRepository.AnyAsync(g => g.Name.Trim().ToLower() == dto.Name.Trim().ToLower());
        if (isExistGroup)
        {
            throw new AlreadyException("Group with the same name already exists");
        }
        _mapper.Map(dto, group);
        _groupRepository.Update(group);
        await _groupRepository.SaveChangesAsync();
        return new();
    }
    private async Task<string> GenerateUniqueCodeAsync()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var rnd = new Random();

        for (int attempt = 0; attempt < 30; attempt++)
        {
            var code = new string(Enumerable.Range(0, 7)
                .Select(_ => chars[rnd.Next(chars.Length)])
                .ToArray());

            var exists = await _groupRepository.AnyAsync(g => g.Code == code);
            if (!exists) return code;
        }

        throw new Exception("Could not generate unique group code.");
    }
}
