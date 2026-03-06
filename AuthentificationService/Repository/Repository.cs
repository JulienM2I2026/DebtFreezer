using AuthentificationService.Data;
using AuthentificationService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthentificationService.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private UserDbContext _db;
        private readonly DbSet<T> _set;

        public Repository(UserDbContext db)
        {
            _db = db;
            _set = db.Set<T>();
        }

        public async Task<T?> GetByIdAsync(object id)
        => await _set.FindAsync(id);

        public async Task<List<T>> GetAllAsync()
            => await _set.ToListAsync();

        public IQueryable<T> Query()
            => _set.AsQueryable();

        public async Task AddAsync(T entity)
            => await _set.AddAsync(entity);

        public void Update(T entity)
            => _set.Update(entity);

        public void Delete(T entity)
            => _set.Remove(entity);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}