using PaymentService.Dtos;

namespace PaymentService.Services;

public interface IPaymentService
{
    Task<List<PaymentDto>> GetAllAsync(Guid userId);
    Task<List<PaymentDto>> GetByDebtIdAsync(int debtId);
    Task<PaymentDto?> GetByIdAsync(int id);
    Task<PaymentDto> CreateAsync(CreatePaymentDto dto);
    Task<PaymentDto?> UpdateAsync(int id, UpdatePaymentDto dto);
    Task<bool> DeleteAsync(int id);
    Task DeleteByDebtIdAsync(int debtId);
}
