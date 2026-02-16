using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KhosuRoom.DataAccess.Configurations;

internal class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {

        builder.Property(x => x.Comment).HasMaxLength(2000);
        builder.Property(x => x.Feedback).HasMaxLength(2000);

        
        builder.HasIndex(x => new { x.AssignmentId, x.StudentId }).IsUnique();

       
        builder.HasOne(s => s.Student)
            .WithMany(u => u.Submissions)
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        
        builder.HasOne(s => s.GradedByTeacher)
            .WithMany(u => u.GradedSubmissions)
            .HasForeignKey(s => s.GradedByTeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}