using AutoMapper;
using KhosuRoom.Business.Dtos.NotificationDtos;

namespace KhosuRoom.Business.Profiles;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, GetNotificationDto>();
    }
}