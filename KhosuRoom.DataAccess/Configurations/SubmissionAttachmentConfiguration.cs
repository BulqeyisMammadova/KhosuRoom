using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KhosuRoom.DataAccess.Configurations;

internal class SubmissionAttachmentConfiguration : IEntityTypeConfiguration<SubmissionAttachment>
{
    public void Configure(EntityTypeBuilder<SubmissionAttachment> builder)
    {
        builder.Property(x => x.FileUrl)
               .IsRequired()
               .HasMaxLength(1000);

        builder.Property(x => x.FileName)
               .IsRequired()
               .HasMaxLength(255);
        builder.Property(x => x.UploadedDate)
                     .IsRequired();

        builder.HasOne(x => x.Submission)
               .WithMany(s => s.SubmissionAttachments)
               .HasForeignKey(x => x.SubmissionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.SubmissionId);

       
    }
}