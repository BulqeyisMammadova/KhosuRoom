namespace KhosuRoom.Business.Dtos.UserDtos;

public class AdminUpdateUserDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
