using KhosuRoom.Core.Entities.Common;
using System.Linq.Expressions;

namespace KhosuRoom.DataAccess.Repository.Abstarctions.Generic;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> GetAll();
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<int> SaveChangesAsync();
}
