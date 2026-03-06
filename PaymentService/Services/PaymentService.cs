using PaymentService.Dtos;
using PaymentService.Models;
using PaymentService.Repository;

namespace PaymentService.Services;

public class PaymentService : IPaymentService
{
    private readonly IRepository<Payment> _paymentRepository;

    public PaymentService(IRepository<Payment> paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<List<PaymentDto>> GetAllAsync()
    {
        var payments = await _paymentRepository.GetAllAsync();

        return payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            DebtId = p.DebtId,
            Amount = p.Amount,
            PaymentDate = p.PaymentDate,
            Notes = p.Notes
        }).ToList();
    }

    public async Task<PaymentDto?> GetByIdAsync(int id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);

        if (payment == null)
            return null;

        return new PaymentDto
        {
            Id = payment.Id,
            DebtId = payment.DebtId,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            Notes = payment.Notes
        };
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto)
    {
        var payment = new Payment
        {
            DebtId = dto.DebtId,
            Amount = dto.Amount,
            PaymentDate = dto.PaymentDate ?? DateTime.UtcNow,
            Notes = dto.Notes
        };

        await _paymentRepository.AddAsync(payment);
        await _paymentRepository.SaveChangesAsync();

        return new PaymentDto
        {
            Id = payment.Id,
            DebtId = payment.DebtId,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            Notes = payment.Notes
        };
    }

    public async Task<PaymentDto?> UpdateAsync(int id, UpdatePaymentDto dto)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);

        if (payment == null)
            return null;

        if (dto.Amount.HasValue)
            payment.Amount = dto.Amount.Value;

        if (dto.PaymentDate.HasValue)
            payment.PaymentDate = dto.PaymentDate.Value;

        if (dto.Notes != null)
            payment.Notes = dto.Notes;

        await _paymentRepository.UpdateAsync(payment);
        await _paymentRepository.SaveChangesAsync();

        return new PaymentDto
        {
            Id = payment.Id,
            DebtId = payment.DebtId,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            Notes = payment.Notes
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);

        if (payment == null)
            return false;

        await _paymentRepository.DeleteAsync(payment);
        await _paymentRepository.SaveChangesAsync();

        return true;
    }

    public Task<List<PaymentDto>> GetByDebtIdAsync(int debtId)
    {
        throw new NotImplementedException();
    }
}
