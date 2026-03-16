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
        Task<DebtSend?> Patch(int id, UpdateDebtDto dto);
        Task<bool> Delete(int id);
        Task<PagedResult<DebtSend>> GetPagedAsync(DebtQueryDto query);
        Task<DebtSummaryDto> GetDebtSummary(Guid userId);
        Task<List<DebtByMonthDto>> GetDebtByMonth(Guid userId);
    }
}
