using DebtService.Models;

namespace DebtService.Repository
{
    public interface IRepository<T>
    {

        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<T> CreateAsync(T entity);
        Task<List<Debt>> GetByUserIdAsync(Guid userId);
        Task<Debt?> GetByUserAndDebtIdAsync(Guid userId, int debtId);
        Task<bool> UpdateAsync(T entity);
    }
}
