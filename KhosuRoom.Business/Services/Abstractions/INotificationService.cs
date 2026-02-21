using KhosuRoom.Business.Dtos.NotificationDtos;
using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Core.Enums;

namespace KhosuRoom.Business.Services.Abstractions;

public interface INotificationService
{
    Task<ResultDto<IEnumerable<GetNotificationDto>>> GetMyNotificationsAsync(int take = 50);
    Task<ResultDto<UnreadCountDto>> GetUnreadCountAsync();
    Task<ResultDto> MarkAsReadAsync(Guid notificationId);


    Task CreateForUsersAsync(
     IEnumerable<Guid> userIds,
     string title,
     string message,
     NotificationType type,
     Guid groupId,
     string? redirectUrl = null,
     Guid? senderUserId = null);

}

