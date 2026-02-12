using AutoMapper;
using KhosuRoom.Business.Dtos.GroupDtos;

namespace KhosuRoom.Business.Profiles;

internal class GroupProfile:Profile
{
    public GroupProfile()
    {
        CreateMap<GroupMember, GroupMemberItemDto>()
           .ForMember(d => d.FullName,
               o => o.MapFrom(s => s.User.FirstName + " " + s.User.LastName));
        CreateMap<Group, GroupGetDto>().ReverseMap();
        CreateMap<Group,GroupCreateDto>().ReverseMap();
        CreateMap<Group,GroupUpdateDto>().ReverseMap();


    }
}
