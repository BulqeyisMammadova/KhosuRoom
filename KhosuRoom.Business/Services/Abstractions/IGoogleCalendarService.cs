namespace KhosuRoom.Business.Services.Abstractions;

public interface IGoogleCalendarService
{
    string GetAuthUrl(Guid groupId);
    Task<string> ExchangeCodeAndCreateMeetLinkAsync(string code, Guid groupId);
    Task<string?> CreateMeetLinkWithStoredTokenAsync(Guid groupId);
}
