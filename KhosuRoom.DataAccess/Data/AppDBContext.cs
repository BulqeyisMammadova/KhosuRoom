using KhosuRoom.Core.Entities;
using KhosuRoom.Core.Entities.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KhosuRoom.DataAccess.Data;

internal class AppDBContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AppDBContext(DbContextOptions options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AppDBContext).Assembly);
        base.OnModelCreating(builder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities=  this.ChangeTracker.Entries<BaseAutitableEntity>().ToList();
        foreach (var entry in entities)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreateDate = DateTime.UtcNow;
                    entry.Entity.CreateBy = "Admin";
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdateDate = DateTime.UtcNow;
                    entry.Entity.UpdateBy = "Admin";
                    break;
                case EntityState.Deleted:
                    entry.Entity.DeleteDate = DateTime.UtcNow;
                    entry.Entity.DeleteBy = "Admin";
                    entry.Entity.IsDeleted = true;
                    entry.State = EntityState.Modified;
                    break;

            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
   public DbSet<Group> Groups { get; set; } = null!;
   public DbSet<GroupMember> GroupMembers { get; set; } = null!;

}
