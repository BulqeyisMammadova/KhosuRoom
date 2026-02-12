using KhosuRoom.Core.Entities.Common;
using KhosuRoom.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace KhosuRoom.DataAccess.Interceptors;

internal class BaseAuditableInterceptor:SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableDatas(eventData);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateAuditableDatas(eventData);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    private static void UpdateAuditableDatas(DbContextEventData eventData)
    {
        if (eventData.Context is AppDBContext context)
        {
            var entities = context.ChangeTracker.Entries<BaseAutitableEntity>().ToList();
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
        }
    }
}
