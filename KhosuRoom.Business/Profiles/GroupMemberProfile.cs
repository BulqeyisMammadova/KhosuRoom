using AutoMapper;
using KhosuRoom.Business.Dtos.GroupMemberDtos;

namespace KhosuRoom.Business.Profiles;

internal class GroupMemberProfile:Profile
{
    public GroupMemberProfile()
    {
        CreateMap<GroupMember, GroupMemberItemDto>()
             .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId))
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.User.FirstName + " " + s.User.LastName))
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));
    }
}
