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
    private readonly IConfiguration _config;

    public PaymentService(IRepository<Payment> paymentRepository, IConfiguration config)
    {
        _paymentRepository = paymentRepository;

        _config = config;
        var debtServiceUrl = _config["ServiceUrls:DebtService"];

        _client = new Client<DebtSend, DebtReceive>($"{debtServiceUrl}/api/v1/Debt/");
        _createClient = new Client<bool, double>($"{debtServiceUrl}/api/v1/Debt/");

    }

    public async Task<List<PaymentDto>> GetAllAsync(Guid userId)
    {
        var payments = await _paymentRepository.GetAllAsync();

        List<PaymentDto> paymentSends = new List<PaymentDto>();
        foreach (Payment payment in payments)
        {
            var dto = await EntityToDto(payment);
            if (dto.Debt == null || dto.Debt.UserId == userId)
                paymentSends.Add(dto);
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
        bool response = await _createClient.PostRequest(dto.DebtId.ToString(), (double)dto.Amount);
        if (!response)
            throw new Exception("Erreur lors de la mise à jour de la dette");

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

    public async Task<List<PaymentDto>> GetByDebtIdAsync(int debtId)
    {
        var payments = await _paymentRepository.FindAsync(p => p.DebtId == debtId);
        var result = new List<PaymentDto>();
        foreach (var payment in payments)
            result.Add(await EntityToDto(payment));
        return result;
    }

    public async Task DeleteByDebtIdAsync(int debtId)
    {
        var payments = await _paymentRepository.FindAsync(p => p.DebtId == debtId);
        foreach (var payment in payments)
            await _paymentRepository.DeleteAsync(payment);
        if (payments.Count > 0)
            await _paymentRepository.SaveChangesAsync();
    }

    private async Task<PaymentDto> EntityToDto(Payment payment)
    {
        PaymentDto send = new PaymentDto() {
            Id = payment.Id,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            Notes = payment.Notes
        };
        try
        {
            send.Debt = await _client.GetRequest(payment.DebtId.ToString());
        }
        catch
        {
            send.Debt = null;
        }
        return send;
    }
}
