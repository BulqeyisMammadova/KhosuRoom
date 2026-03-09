using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using KhosuRoom.Business.Exceptions;
using KhosuRoom.Business.Services.Abstractions;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using Microsoft.Extensions.Configuration;

namespace KhosuRoom.Business.Services.Implementations;

public class GoogleCalendarService : IGoogleCalendarService
{
    private readonly IConfiguration _configuration;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;
    private readonly IGroupRepository _groupRepository;

    public GoogleCalendarService(IConfiguration configuration, IGroupRepository groupRepository)
    {
        _configuration = configuration;
        _clientId = _configuration["GoogleCalendar:ClientId"] ?? throw new ArgumentNullException("GoogleCalendar:ClientId is missing");
        _clientSecret = _configuration["GoogleCalendar:ClientSecret"] ?? throw new ArgumentNullException("GoogleCalendar:ClientSecret is missing");
        _redirectUri = _configuration["GoogleCalendar:RedirectUri"] ?? throw new ArgumentNullException("GoogleCalendar:RedirectUri is missing");
        _groupRepository = groupRepository;
    }

    public string GetAuthUrl(Guid groupId)
    {
        string encodedRedirectUri = Uri.EscapeDataString(_redirectUri);
        // access_type=offline → Google returns a refresh_token
        // prompt=consent      → Ensures refresh_token is always returned on first auth
        string authUrl = $"https://accounts.google.com/o/oauth2/v2/auth" +
                         $"?client_id={_clientId}" +
                         $"&redirect_uri={encodedRedirectUri}" +
                         $"&response_type=code" +
                         $"&scope=https://www.googleapis.com/auth/calendar.events" +
                         $"&access_type=offline" +
                         $"&prompt=consent" +
                         $"&state={groupId}";
        return authUrl;
    }

    /// <summary>
    /// Creates a Google Meet link for the group.
    /// If the group already has a saved refresh token, reuses it silently (no Google login).
    /// Otherwise, exchanges the OAuth code for tokens, saves the refresh token, and creates the link.
    /// </summary>
    public async Task<string> ExchangeCodeAndCreateMeetLinkAsync(string code, Guid groupId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId) ?? throw new NotFoundExceptions("Group not found");

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret
            },
            Scopes = new[] { CalendarService.Scope.CalendarEvents }
        });

        // Exchange OAuth authorization code for tokens
        var token = await flow.ExchangeCodeForTokenAsync("user", code, _redirectUri, CancellationToken.None);

        // Persist refresh token so future calls don't need Google login
        if (!string.IsNullOrEmpty(token.RefreshToken))
        {
            group.GoogleRefreshToken = token.RefreshToken;
        }

        var credential = new UserCredential(flow, "user", token);
        var meetLink = await CreateMeetEventAsync(credential, group.Name);

        // Save Meet link and refresh token
        group.MeetLink = meetLink;
        _groupRepository.Update(group);
        await _groupRepository.SaveChangesAsync();

        return meetLink;
    }

    /// <summary>
    /// Creates a new Meet link using the stored refresh token — no Google login required.
    /// Returns null if no refresh token is stored (caller should redirect to auth).
    /// </summary>
    public async Task<string?> CreateMeetLinkWithStoredTokenAsync(Guid groupId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId) ?? throw new NotFoundExceptions("Group not found");

        if (string.IsNullOrEmpty(group.GoogleRefreshToken))
            return null; // No stored token — need OAuth flow

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret
            },
            Scopes = new[] { CalendarService.Scope.CalendarEvents }
        });

        // Build token from stored refresh token
        var token = new TokenResponse
        {
            RefreshToken = group.GoogleRefreshToken,
            // No access token — the library will automatically refresh it
        };

        var credential = new UserCredential(flow, "user", token);

        try
        {
            var meetLink = await CreateMeetEventAsync(credential, group.Name);

            // Update the refreshed token if it changed
            if (!string.IsNullOrEmpty(credential.Token.RefreshToken) && credential.Token.RefreshToken != group.GoogleRefreshToken)
            {
                group.GoogleRefreshToken = credential.Token.RefreshToken;
            }

            group.MeetLink = meetLink;
            _groupRepository.Update(group);
            await _groupRepository.SaveChangesAsync();

            return meetLink;
        }
        catch
        {
            // Refresh token might be expired/revoked — clear it so we fall back to OAuth
            group.GoogleRefreshToken = null;
            _groupRepository.Update(group);
            await _groupRepository.SaveChangesAsync();
            return null;
        }
    }

    private async Task<string> CreateMeetEventAsync(UserCredential credential, string groupName)
    {
        var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "KhosuRoom",
        });

        Event newEvent = new Event()
        {
            Summary = $"{groupName} - Online Class",
            Description = $"Auto-generated Google Meet link for {groupName}",
            Start = new EventDateTime()
            {
                DateTimeDateTimeOffset = DateTimeOffset.UtcNow,
                TimeZone = "UTC",
            },
            End = new EventDateTime()
            {
                DateTimeDateTimeOffset = DateTimeOffset.UtcNow.AddYears(1),
                TimeZone = "UTC",
            },
            ConferenceData = new ConferenceData
            {
                CreateRequest = new CreateConferenceRequest
                {
                    RequestId = Guid.NewGuid().ToString(),
                    ConferenceSolutionKey = new ConferenceSolutionKey { Type = "hangoutsMeet" }
                }
            }
        };

        var request = service.Events.Insert(newEvent, "primary");
        request.ConferenceDataVersion = 1;

        var createdEvent = await request.ExecuteAsync();

        if (createdEvent.ConferenceData?.EntryPoints == null)
            throw new Exception("Failed to generate Google Meet link.");

        var meetLink = createdEvent.ConferenceData.EntryPoints.FirstOrDefault(e => e.EntryPointType == "video")?.Uri;

        if (string.IsNullOrEmpty(meetLink))
            throw new Exception("Meet link entry point was not found.");

        return meetLink;
    }
}
