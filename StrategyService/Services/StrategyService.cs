using StrategyService.Dtos;
using StrategyService.Dtos.External;
using StrategyService.Enums;
using StrategyService.Models;
using StrategyService.Repository;
using StrategyService.RestClient;
using System.Text.Json;

namespace StrategyService.Services
{
    public class StrategyService : IStrategyService
    {
        private readonly IRepository<RepaymentPlan> _repository;
        private readonly ServiceClient _serviceClient;

        public StrategyService(IRepository<RepaymentPlan> repository, ServiceClient serviceClient)
        {
            _repository = repository;
            _serviceClient = serviceClient;
        }

        // ------------------------------------------------------------------
        // Calcul et sauvegarde du plan — les données viennent des autres services
        // ------------------------------------------------------------------
        public async Task<RepaymentPlanDto> CalculateAndSaveAsync(CalculateStrategyDto dto)
        {
            // 1. Récupérer les dettes ACTIVES de l'utilisateur depuis DebtService
            var userDebts = await _serviceClient.GetListAsync<ExternalDebtDto>("DebtService", $"/api/v1/Debt?userId={dto.UserId}");
            userDebts = userDebts.Where(d => d.Status == 0).ToList(); // ACTIVE = 0

            if (!userDebts.Any())
                throw new InvalidOperationException($"Aucune dette active trouvée pour l'utilisateur {dto.UserId} dans DebtService.");

            // 2. Utiliser le RemainingAmount directement depuis DebtService
            //    (mis à jour automatiquement à chaque paiement enregistré)
            var debtInputs = userDebts
                .Where(d => d.RemainingAmount > 0.01)
                .Select(d => new DebtInputDto
                {
                    Creditor = d.Creditor,
                    RemainingAmount = d.RemainingAmount,
                    InterestRate = d.InterestRate
                })
                .ToList();

            if (!debtInputs.Any())
                throw new InvalidOperationException($"Toutes les dettes de l'utilisateur {dto.UserId} sont déjà remboursées.");

            // 3. Lancer l'algorithme Avalanche / Snowball
            var (totalMonths, totalInterest, priorityOrder) = Calculate(dto.StrategyType, dto.MonthlyBudget, debtInputs);

            // 4. Persister le plan calculé
            var plan = new RepaymentPlan
            {
                UserId = dto.UserId,
                StrategyType = dto.StrategyType,
                MonthlyBudget = dto.MonthlyBudget,
                TotalMonths = totalMonths,
                TotalInterestPaid = Math.Round(totalInterest, 2),
                DebtFreeDate = DateTime.UtcNow.AddMonths(totalMonths),
                CreatedAt = DateTime.UtcNow,
                DebtPriorityJson = JsonSerializer.Serialize(priorityOrder)
            };

            await _repository.AddAsync(plan);
            await _repository.SaveChangesAsync();

            return MapToDto(plan, priorityOrder);
        }

        public async Task<List<RepaymentPlanDto>> GetAllAsync()
        {
            var plans = await _repository.GetAllAsync();
            return plans.Select(p => MapToDto(p, DeserializePriority(p.DebtPriorityJson))).ToList();
        }

        public async Task<List<RepaymentPlanDto>> GetByUserIdAsync(Guid userId)
        {
            var plans = await _repository.FindAsync(p => p.UserId == userId);
            return plans.Select(p => MapToDto(p, DeserializePriority(p.DebtPriorityJson))).ToList();
        }

        public async Task<RepaymentPlanDto?> GetByIdAsync(int id)
        {
            var plan = await _repository.GetByIdAsync(id);
            if (plan == null) return null;
            return MapToDto(plan, DeserializePriority(plan.DebtPriorityJson));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var plan = await _repository.GetByIdAsync(id);
            if (plan == null) return false;

            await _repository.DeleteAsync(plan);
            await _repository.SaveChangesAsync();
            return true;
        }

        // ------------------------------------------------------------------
        // Algorithme Avalanche / Snowball
        // ------------------------------------------------------------------

        private static (int totalMonths, double totalInterest, List<string> priorityOrder) Calculate(
            StrategyType strategyType, double monthlyBudget, List<DebtInputDto> debts)
        {
            // Tri selon la stratégie choisie
            var sorted = strategyType == StrategyType.Avalanche
                ? debts.OrderByDescending(d => d.InterestRate).ToList()
                : debts.OrderBy(d => d.RemainingAmount).ToList();

            var priorityOrder = sorted.Select(d => d.Creditor).ToList();
            var remaining = sorted.Select(d => d.RemainingAmount).ToArray();

            double totalInterest = 0;
            int months = 0;
            const int maxMonths = 1200; // sécurité : max 100 ans

            while (remaining.Any(r => r > 0.01) && months < maxMonths)
            {
                months++;

                // Étape 1 : intérêts mensuels sur chaque dette encore active
                for (int i = 0; i < sorted.Count; i++)
                {
                    if (remaining[i] > 0)
                    {
                        double interest = remaining[i] * (sorted[i].InterestRate / 100.0 / 12.0);
                        remaining[i] += interest;
                        totalInterest += interest;
                    }
                }

                // Étape 2 : appliquer le budget mensuel en ordre de priorité
                double budget = monthlyBudget;
                for (int i = 0; i < sorted.Count && budget > 0; i++)
                {
                    if (remaining[i] > 0)
                    {
                        double payment = Math.Min(budget, remaining[i]);
                        remaining[i] -= payment;
                        budget -= payment;

                        if (remaining[i] < 0.01)
                            remaining[i] = 0;
                    }
                }
            }

            return (months, totalInterest, priorityOrder);
        }

        private static RepaymentPlanDto MapToDto(RepaymentPlan plan, List<string> priorityOrder) => new()
        {
            Id = plan.Id,
            UserId = plan.UserId,
            StrategyType = plan.StrategyType,
            MonthlyBudget = plan.MonthlyBudget,
            TotalMonths = plan.TotalMonths,
            TotalInterestPaid = plan.TotalInterestPaid,
            DebtFreeDate = plan.DebtFreeDate,
            DebtPriorityOrder = priorityOrder,
            CreatedAt = plan.CreatedAt
        };

        private static List<string> DeserializePriority(string json)
        {
            if (string.IsNullOrEmpty(json)) return new List<string>();
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
    }
}
