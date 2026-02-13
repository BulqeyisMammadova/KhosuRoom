namespace KhosuRoom.Business.Dtos;

public class AccessTokenDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiredToken { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiredRefreshToken { get; set; }

}
