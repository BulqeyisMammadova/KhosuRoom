using AutoMapper;
using KhosuRoom.Business.Dtos.NotificationDtos;
using KhosuRoom.Business.Dtos.ResultDtos;
using KhosuRoom.Business.Exceptions;
using KhosuRoom.Business.Services.Abstractions;
using KhosuRoom.Core.Enums;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KhosuRoom.Business.Services.Implementations;

internal class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IHttpContextAccessor _http;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly UserManager<AppUser> _userManager;

    public NotificationService(INotificationRepository notificationRepository, IHttpContextAccessor http, IMapper mapper, IEmailService emailService, UserManager<AppUser> userManager)
    {
        _notificationRepository = notificationRepository;
        _http = http;
        _mapper = mapper;
        _emailService = emailService;
        _userManager = userManager;
    }

    
    private Guid CurrentUserId()
    {
        var user = _http.HttpContext?.User;
        if(user == null) throw new LoginException("Unauthorized");
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new LoginException("Unauthorized");
        return Guid.Parse(userId);
    }


    public async Task<ResultDto<IEnumerable<GetNotificationDto>>> GetMyNotificationsAsync(int take = 20)
    {
        var userId = CurrentUserId();
        if (take == 0) take = 20;
        if (take > 100) take = 50;
        var notifications = await _notificationRepository.GetAll()
            .Where(x => x.UserId == userId)
            .OrderByDescending(n => n.CreateDate)
            .Take(take)
            .ToListAsync();
        var users = _mapper.Map<IEnumerable<GetNotificationDto>>(notifications);
        return new(users);

    }

    public async Task<ResultDto<UnreadCountDto>> GetUnreadCountAsync()
    {
        var userId = CurrentUserId();
        var notificationsCount = await _notificationRepository.GetAll()
            .Where(x=>x.UserId == userId && !x.IsRead)
            .CountAsync();
        return new(new UnreadCountDto { Count = notificationsCount });
    }


    public async Task<ResultDto> MarkAsReadAsync(Guid notificationId)
    {
        var userId = CurrentUserId();
        var notification = await _notificationRepository.GetAll()
            .FirstOrDefaultAsync(x=>x.Id == notificationId &&  x.UserId == userId);
        if(notification == null) throw new NotFoundExceptions("Notification not found");
        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
           await _notificationRepository.SaveChangesAsync();
        }
        return new();
    }

    public async Task CreateForUsersAsync(
    IEnumerable<Guid> userIds,
    string title,
    string message,
    NotificationType type,
    Guid? groupId,
    string? redirectUrl = null)
    {
        var ids = userIds?.Distinct().ToList() ?? [];
        if (ids.Count == 0) return;

        var now = DateTime.UtcNow;

        foreach (var uid in ids)
        {
            var notification = new Notification
            {
                UserId = uid,
                GroupId = (Guid)groupId,   
                Title = title,
                Message = message,
                Type = type,
                RedirectUrl = redirectUrl,
                IsRead = false,
                ReadAt = null,
                CreateDate = now,
                CreateBy = "SYSTEM"
            };

            await _notificationRepository.AddAsync(notification);
        }

        await _notificationRepository.SaveChangesAsync();

        
        var emails = await _userManager.Users
            .Where(u => ids.Contains(u.Id) && u.Email != null)
            .Select(u => u.Email!)
            .ToListAsync();

       
        foreach (var email in emails)
        {
            try
            {
                var subject = $"KhosuRoom: {title}";
                var body = $@"
                <h3>{title}</h3>
                <p>{message}</p>
                {(redirectUrl is not null ? $"<p><a href='{redirectUrl}'>Open</a></p>" : "")}
            ";

                await _emailService.SendEmailAsync(email, subject, body);
            }
            catch
            {
                
            }
        }
    }
}
