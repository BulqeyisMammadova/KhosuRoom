using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KhosuRoom.DataAccess.Configurations;

internal class AttendanceSessionConfiguration : IEntityTypeConfiguration<AttendanceSession>
{
    public void Configure(EntityTypeBuilder<AttendanceSession> builder)
    {
        builder.HasOne(x => x.Group)
          .WithMany(g => g.AttendanceSessions)
          .HasForeignKey(x => x.GroupId)
          .OnDelete(DeleteBehavior.Cascade);

        
        builder.HasOne(x => x.Teacher)
            .WithMany()
            .HasForeignKey(x => x.TeacherId)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.HasIndex(x => new { x.GroupId, x.Date }).IsUnique();

        builder.HasMany(x => x.Records)
            .WithOne(x => x.AttendanceSession)
            .HasForeignKey(x => x.AttendanceSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
