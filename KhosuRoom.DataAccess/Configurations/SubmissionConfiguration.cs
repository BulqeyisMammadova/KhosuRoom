using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KhosuRoom.DataAccess.Configurations;

internal class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.Property(x => x.Text)
               .HasMaxLength(8000);

        builder.HasOne(x => x.Assignment)
               .WithMany(a => a.Submissions)
               .HasForeignKey(x => x.AssignmentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Student)
               .WithMany()
               .HasForeignKey(x => x.StudentId)
               .OnDelete(DeleteBehavior.Restrict);

        
        builder.HasIndex(x => new { x.AssignmentId, x.StudentId })
               .IsUnique();
    }
}