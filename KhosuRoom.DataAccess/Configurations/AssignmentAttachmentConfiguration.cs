using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KhosuRoom.DataAccess.Configurations;

internal class AssignmentAttachmentConfiguration : IEntityTypeConfiguration<AssignmentAttachment>
{
    public void Configure(EntityTypeBuilder<AssignmentAttachment> builder)
    {
        builder.Property(x => x.FileUrl)
               .IsRequired()
               .HasMaxLength(1000);

        builder.Property(x => x.FileName)
               .IsRequired()
               .HasMaxLength(256);

        builder.HasOne(x => x.Assignment)
               .WithMany(a => a.Attachments)
               .HasForeignKey(x => x.AssignmentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.AssignmentId);
    }
}
