using DebtService.Dtos;
using DebtService.Models;
using DebtService.Repository;
using System.Text.Json;

namespace DebtService.Services
{
    public class Service : IService<DebtSend, DebtReceive>
    {

        private readonly IRepository<Debt> _repository;
        private readonly string _paymentServiceUrl;

        public Service(IRepository<Debt> repository, IConfiguration config)
        {
            _repository = repository;
            // _paymentServiceUrl = config["ServiceUrls:PaymentService"] ?? "http://localhost:5001";
            _paymentServiceUrl = config["ServiceUrls:PaymentService"] ?? "http://localhost:5272";
        }



        public async Task<DebtSend> Create(DebtReceive receive)
        {
            receive.RemainingAmount = receive.OriginalAmount;
            Debt debt = DtoToEntity(receive);
            await _repository.CreateAsync(debt);
            return await EntityToDto(debt);
        }

        public async Task<List<DebtSend>> GetAll()
        {
            List<Debt> debts = await _repository.GetAllAsync();
            List<DebtSend> debtSends = new List<DebtSend>();
            foreach (Debt send in debts)
            {
                debtSends.Add(await EntityToDto(send));
            }
            return debtSends;
        }


        public async Task<List<DebtSend>> GetAllByUserId(Guid userId)
        {
            var debts = await _repository.GetByUserIdAsync(userId);
            Console.WriteLine(JsonSerializer.Serialize(debts)); // voir toutes les propriétés
            return debts.Select(d => new DebtSend
            {
                Id = d.Id,
                UserId = d.UserId,
                Creditor = d.Creditor,
                InterestRate = d.InterestRate,
                Type = d.Type,
                OriginalAmount = d.OriginalAmount,
                RemainingAmount = d.RemainingAmount,
                Status = d.Status,
                DueDate = d.DueDate,
                LastPaymentDate = d.LastPaymentDate,
                AccruedInterest = ComputeAccruedInterest(d),
            }).ToList();
        }


        public async Task<DebtSend> GetById(int id)
        {
            Debt? debt = await _repository.GetByIdAsync(id);

            if (debt == null)
                throw new Exception("Debt not found");

            return await EntityToDto(debt);
        }

        public async Task<DebtSend> GetByUserAndDebtId(Guid userId, int debtId)
        {
            Debt? debt = await _repository.GetByUserAndDebtIdAsync(userId, debtId);
            if (debt == null)
                throw new Exception("Debt not found");

            return await EntityToDto(debt);
        }

        public async Task<bool> UpdateDebtAmount(int id, double amount)
        {
            Debt? debt = await _repository.GetByIdAsync(id);
            if (debt == null || debt.Status == 1 || debt.RemainingAmount < amount)
                return false;
            debt.RemainingAmount -= amount;
            debt.LastPaymentDate = DateTime.UtcNow;
            if(debt.RemainingAmount == 0)
                debt.Status = 1;

            await _repository.UpdateAsync(debt);
            return true;
        }


        public async Task<DebtSend?> Patch(int id, UpdateDebtDto dto)
        {
            Debt? debt = await _repository.GetByIdAsync(id);
            if (debt == null) return null;

            if (dto.Creditor != null) debt.Creditor = dto.Creditor;
            if (dto.OriginalAmount.HasValue) debt.OriginalAmount = dto.OriginalAmount.Value;
            if (dto.RemainingAmount.HasValue) debt.RemainingAmount = dto.RemainingAmount.Value;
            if (dto.InterestRate.HasValue) debt.InterestRate = dto.InterestRate.Value;
            if (dto.DueDate.HasValue) debt.DueDate = dto.DueDate.Value;
            if (dto.Type.HasValue) debt.Type = dto.Type.Value;
            if (dto.Status.HasValue) debt.Status = dto.Status.Value;

            await _repository.UpdateAsync(debt);
            return await EntityToDto(debt);
        }

        public async Task<bool> Delete(int id)
        {
            var ok = await _repository.DeleteAsync(id);
            if (ok)
            {
                try
                {
                    using var http = new HttpClient();
                    await http.DeleteAsync($"{_paymentServiceUrl}/api/v1/Payment/by-debt/{id}");
                }
                catch { /* ne pas bloquer si PaymentService indisponible */ }
            }
            return ok;
        }

        private Debt DtoToEntity(DebtReceive receive)
        {
            return new Debt() { UserId = receive.UserId, Creditor = receive.Creditor, OriginalAmount = receive.OriginalAmount, InterestRate = receive.InterestRate, DueDate = receive.DueDate, Type = receive.Type, Status = receive.Status, RemainingAmount = receive.RemainingAmount };
        }


        private async Task<DebtSend> EntityToDto(Debt debt)
        {
            DebtSend send = new DebtSend()
            {
                Id = debt.Id,
                UserId = debt.UserId,
                Creditor = debt.Creditor,
                OriginalAmount = debt.OriginalAmount,
                InterestRate = debt.InterestRate,
                DueDate = debt.DueDate,
                Type = debt.Type,
                Status = debt.Status,
                RemainingAmount = debt.RemainingAmount,
                LastPaymentDate = debt.LastPaymentDate,
                AccruedInterest = ComputeAccruedInterest(debt),
            };

            return send;
        }

        public async Task<PagedResult<DebtSend>> GetPagedAsync(DebtQueryDto query)
        {
            var (items, total) = await _repository.GetPagedAsync(query);
            return new PagedResult<DebtSend>
            {
                Items = items.Select(d => new DebtSend
                {
                    Id = d.Id,
                    UserId = d.UserId,
                    Creditor = d.Creditor,
                    InterestRate = d.InterestRate,
                    Type = d.Type,
                    OriginalAmount = d.OriginalAmount,
                    RemainingAmount = d.RemainingAmount,
                    Status = d.Status,
                    DueDate = d.DueDate,
                    LastPaymentDate = d.LastPaymentDate,
                    AccruedInterest = ComputeAccruedInterest(d),
                }).ToList(),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize,
            };
        }

        public async Task<DebtSummaryDto> GetDebtSummary(Guid userId)
        {
            var debts = await _repository.GetByUserIdAsync(userId);
            return new DebtSummaryDto
            {
                TotalOriginalAmount = debts.Sum(d => d.OriginalAmount),
                TotalRemainingAmount = debts.Sum(d => d.RemainingAmount),
                TotalPaidAmount = debts.Sum(d => d.OriginalAmount - d.RemainingAmount),
                ActiveDebtsCount = debts.Count(d => d.Status == 0),
                PaidDebtsCount = debts.Count(d => d.Status == 1),
                AverageInterestRate = debts.Count > 0 ? debts.Average(d => d.InterestRate) : 0,
                TotalAccruedInterest = debts.Sum(d => ComputeAccruedInterest(d)),
            };
        }

        public async Task<List<DebtByMonthDto>> GetDebtByMonth(Guid userId)
        {
            var debts = await _repository.GetByUserIdAsync(userId);
            return debts
                .GroupBy(d => new { d.DueDate.Year, d.DueDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new DebtByMonthDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    DebtCount = g.Count(),
                    TotalOriginalAmount = g.Sum(d => d.OriginalAmount),
                })
                .ToList();
        }

        private static double ComputeAccruedInterest(Debt debt)
        {
            if (debt.LastPaymentDate == null || debt.Status == 1)
                return 0d;
            var days = (DateTime.UtcNow - debt.LastPaymentDate.Value).TotalDays;
            return Math.Round(debt.RemainingAmount * (debt.InterestRate / 100.0 / 365.0) * days, 2);
        }
    }
}
