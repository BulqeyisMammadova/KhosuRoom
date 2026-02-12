using KhosuRoom.Core.Entities.Common;
using KhosuRoom.DataAccess.Data;
using KhosuRoom.DataAccess.Repository.Abstarctions.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace KhosuRoom.DataAccess.Repository.Implementations.Generic;


internal class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly AppDBContext _context;

    public Repository(AppDBContext context)
    {
        _context = context;
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);

    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        var result = await _context.Set<T>().AnyAsync(predicate);
        return result;
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public IQueryable<T> GetAll()
    {
        return _context.Set<T>();
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        var entity = await _context.Set<T>().FirstOrDefaultAsync(predicate);
        return entity;
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        return entity;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }
}