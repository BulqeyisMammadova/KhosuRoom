using Microsoft.AspNetCore.Identity;

namespace KhosuRoom.Core.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? ProfileImageUrl { get; set; }

    public bool IsActive { get; set; } = true;
   public ICollection<GroupMember> GroupMembers { get; set; } = [];
}
