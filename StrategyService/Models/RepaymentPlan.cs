using StrategyService.Enums;
using System.ComponentModel.DataAnnotations;

namespace StrategyService.Models
{
    public class RepaymentPlan
    {
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public StrategyType StrategyType { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public double MonthlyBudget { get; set; }

        public int TotalMonths { get; set; }

        public double TotalInterestPaid { get; set; }

        public DateTime DebtFreeDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Ordre de priorité des dettes sérialisé en JSON
        public string DebtPriorityJson { get; set; } = string.Empty;
    }
}
