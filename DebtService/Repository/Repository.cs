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

        public async Task<List<Debt>> GetByUserIdAsync(Guid userId)
        {
            return await _db.Debts
                .Where(d => d.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Debt>> GetAllAsync()
        {
            return await _db.Debts.ToListAsync();
        }

        public async Task<Debt?> GetByIdAsync(int id)
        {
            return await _db.Debts.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(Debt entity)
        {
            _db.Update(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Debt?> GetByUserAndDebtIdAsync(Guid userId, int debtId)
        {
            return await _db.Debts
                .Where(d => d.UserId == userId && d.Id == debtId)
                .FirstOrDefaultAsync();  // retourne null si aucune correspondance
        }

    }
}
