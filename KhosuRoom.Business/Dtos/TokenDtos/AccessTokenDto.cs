namespace KhosuRoom.Business.Dtos;

public class AccessTokenDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiredToken { get; set; }
}
