using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KhosuRoom.DataAccess.Configurations;

internal class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(x => x.FirstName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.LastName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.ProfileImageUrl)
               .HasMaxLength(500);
    }
}


