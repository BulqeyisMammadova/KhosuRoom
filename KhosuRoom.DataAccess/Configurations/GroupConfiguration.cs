using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KhosuRoom.DataAccess.Configurations;

internal class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Code)
               .IsRequired()
               .HasMaxLength(20);

        builder.HasIndex(x => x.Code)
               .IsUnique();
    }
}
