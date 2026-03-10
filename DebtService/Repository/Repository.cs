using DebtService.Data;
using DebtService.Models;
using Microsoft.EntityFrameworkCore;

namespace DebtService.Repository
{
    public class Repository : IRepository<Debt>
    {

        private readonly AppDbContext _db;

        public Repository(AppDbContext appDb)
        {
            _db = appDb;
        }
        public async Task<Debt> CreateAsync(Debt entity)
        {
            _db.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }


        public async Task<List<Debt>> GetAllAsync()
        {
            return await _db.Debts.ToListAsync();
        }

        public async Task<Debt?> GetByIdAsync(int id)
        {
            return await _db.Debts.FindAsync(id);
        }
    }
}
