using KhosuRoom.Core.Entities;
using KhosuRoom.Core.Entities.Common;
using KhosuRoom.DataAccess.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KhosuRoom.DataAccess.Data;

internal class AppDBContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    private readonly BaseAuditableInterceptor _baseAuditableInterceptor;


    public AppDBContext(DbContextOptions options, BaseAuditableInterceptor baseAuditableInterceptor) : base(options)
    {
        _baseAuditableInterceptor = baseAuditableInterceptor;
    }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDBContext).Assembly);
        modelBuilder.Entity<Group>().HasQueryFilter(g => !g.IsDeleted);
        modelBuilder.Entity<Assignment>().HasQueryFilter(a => !a.IsDeleted);
        modelBuilder.Entity<Submission>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<Notification>().HasQueryFilter(n => !n.IsDeleted);
        modelBuilder.Entity<ChatMessage>().HasQueryFilter(n => !n.IsDeleted);

    }

    override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_baseAuditableInterceptor);
        base.OnConfiguring(optionsBuilder);
    }



    public DbSet<Group> Groups { get; set; } = null!;
   public DbSet<GroupMember> GroupMembers { get; set; } = null!;
    public DbSet<Assignment> Assignments { get; set; } = null!;
    public DbSet<AssignmentAttachment> AssignmentAttachments { get; set; } = null!;
    public DbSet<Submission> Submissions { get; set; } = null!;
    public DbSet<SubmissionAttachment> SubmissionAttachments { get; set; } = null!;
    public DbSet<AttendanceSession> AttendanceSessions { get; set; } = null!;
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<ChatMessage> ChatMessages { get; set; } = null!;   

}
