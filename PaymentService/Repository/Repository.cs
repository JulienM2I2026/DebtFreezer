using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using PaymentService.Data;

namespace PaymentService.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public Task<List<T>> GetAllAsync()
        => _dbSet.ToListAsync();

    public Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => _dbSet.Where(predicate).ToListAsync();

    public Task<T?> GetByIdAsync(int id)
        => _dbSet.FindAsync(id).AsTask();

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
}
