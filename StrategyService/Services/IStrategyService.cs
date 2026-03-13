using StrategyService.Dtos;

namespace StrategyService.Services
{
    public interface IStrategyService
    {
        Task<RepaymentPlanDto> CalculateAndSaveAsync(CalculateStrategyDto dto);
        Task<List<RepaymentPlanDto>> GetAllAsync();
        Task<List<RepaymentPlanDto>> GetByUserIdAsync(Guid userId);
        Task<RepaymentPlanDto?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
