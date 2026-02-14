using AutoMapper;
using KhosuRoom.Business.Dtos.UserDtos;

namespace KhosuRoom.Business.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<AdminCreateUserDto, AppUser>();
        CreateMap<AdminUpdateUserDto, AppUser>()
            .ForMember(d => d.Id, o => o.Ignore());

        CreateMap<AppUser, UserGetDto>();

        CreateMap<MeUpdateProfileImageDto, AppUser>();
    }
}
