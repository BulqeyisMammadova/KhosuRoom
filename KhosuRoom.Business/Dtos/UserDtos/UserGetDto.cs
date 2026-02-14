namespace KhosuRoom.Business.Dtos.UserDtos;

public class UserGetDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; }

    public IList<string> Roles { get; set; } = new List<string>();
}
