using PaymentService.Dtos;
using PaymentService.Models;
using PaymentService.Repository;
using PaymentService.RestClient;
using System.Diagnostics;
using System.Text.Json;

namespace PaymentService.Services;

public class PaymentService : IPaymentService
{
    private readonly IRepository<Payment> _paymentRepository;
    private readonly Client<DebtSend, DebtReceive> _client;
    private readonly Client<bool, double> _createClient;

    public PaymentService(IRepository<Payment> paymentRepository)
    {
        _paymentRepository = paymentRepository;
        _client = new Client<DebtSend, DebtReceive>("http://localhost:5193/api/Debt/");
        _createClient = new Client<bool, double>("http://localhost:5193/api/Debt/");
    }

    public async Task<List<PaymentDto>> GetAllAsync()
    {
        var payments = await _paymentRepository.GetAllAsync();

        List<PaymentDto> paymentSends = new List<PaymentDto>();
        foreach (Payment payment in payments)
        {
            paymentSends.Add(await EntityToDto(payment));
        }
        return paymentSends;
    }

    public async Task<PaymentDto?> GetByIdAsync(int id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);

        if (payment == null)
            return null;

        return await EntityToDto(payment);
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto)
    {
        await _createClient.PostRequest(dto.DebtId.ToString(), (double)dto.Amount);
        var payment = new Payment
        {
            DebtId = dto.DebtId,
            Amount = dto.Amount,
            PaymentDate = dto.PaymentDate ?? DateTime.UtcNow,
            Notes = dto.Notes
        };

        await _paymentRepository.AddAsync(payment);
        await _paymentRepository.SaveChangesAsync();

        return await EntityToDto(payment);
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

        return await EntityToDto(payment);
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

    private async Task<PaymentDto> EntityToDto(Payment payment)
    {
        PaymentDto send = new PaymentDto() {
            Id = payment.Id,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            Notes = payment.Notes
        };
        send.Debt = await _client.GetRequest(payment.DebtId.ToString());
        return send;
    }
}
