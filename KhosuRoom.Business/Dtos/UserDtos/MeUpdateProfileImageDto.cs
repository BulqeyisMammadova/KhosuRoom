using Microsoft.AspNetCore.Http;

namespace KhosuRoom.Business.Dtos.UserDtos;

public class MeUpdateProfileImageDto
{
    public IFormFile? ProfileImageUrl { get; set; }
}
