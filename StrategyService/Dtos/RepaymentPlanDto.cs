using StrategyService.Enums;

namespace StrategyService.Dtos
{
    public class RepaymentPlanDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public StrategyType StrategyType { get; set; }
        public double MonthlyBudget { get; set; }
        public int TotalMonths { get; set; }
        public double TotalInterestPaid { get; set; }
        public DateTime DebtFreeDate { get; set; }
        public List<string> DebtPriorityOrder { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
