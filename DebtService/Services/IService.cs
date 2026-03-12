using DebtService.Dtos;

namespace DebtService.Services
{
    public interface IService<Tsend, Treceive>
    {
        Task<Tsend> Create(Treceive receive);
        Task<Tsend> GetById(int id);
        Task<List<Tsend>> GetAll();
        Task<List<DebtSend>> GetAllByUserId(Guid userId);
        Task<DebtSend> GetByUserAndDebtId(Guid userId, int debtId);
        Task<bool> UpdateDebtAmount(int id, double amount);
    }
}
