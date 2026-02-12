using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KhosuRoom.DataAccess.Configurations;

internal class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.HasOne(x => x.User)
                .WithMany(u => u.GroupMembers)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Group)
               .WithMany(g => g.Members)
               .HasForeignKey(x => x.GroupId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.GroupId })
               .IsUnique();

        builder.Property(x => x.Role)
               .IsRequired();
    }
}