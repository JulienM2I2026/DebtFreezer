using DebtService.Data;
using DebtService.Dtos;
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

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.Debts.FindAsync(id);
            if (entity == null) return false;
            _db.Debts.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(List<Debt> Items, int TotalCount)> GetPagedAsync(DebtQueryDto query)
        {
            var q = _db.Debts.Where(d => d.UserId == query.UserId);

            if (query.Status.HasValue)
                q = q.Where(d => d.Status == query.Status.Value);

            if (query.Type.HasValue)
                q = q.Where(d => d.Type == query.Type.Value);

            if (!string.IsNullOrWhiteSpace(query.Creditor))
                q = q.Where(d => d.Creditor.Contains(query.Creditor));

            var total = await q.CountAsync();

            var items = await q
                .OrderBy(d => d.DueDate)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (items, total);
        }

    }
}
