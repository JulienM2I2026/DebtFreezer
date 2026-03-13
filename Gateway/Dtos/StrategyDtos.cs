namespace Gateway.Dtos
{
    // Ce que le frontend envoie pour calculer une stratégie (sans userId, injecté par le Gateway)
    public class CalculateStrategyGatewayDto
    {
        public double MonthlyBudget { get; set; }
        public int StrategyType { get; set; } // 1 = Snowball, 2 = Avalanche
    }

    // Ce que le Gateway envoie au StrategyService (avec userId injecté depuis le token)
    public class CalculateStrategyInternalDto
    {
        public Guid UserId { get; set; }
        public double MonthlyBudget { get; set; }
        public int StrategyType { get; set; }
    }

    // Réponse du StrategyService pour un plan de remboursement
    public class RepaymentPlanDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StrategyType { get; set; }
        public double MonthlyBudget { get; set; }
        public int TotalMonths { get; set; }
        public double TotalInterestPaid { get; set; }
        public DateTime DebtFreeDate { get; set; }
        public List<string> DebtPriorityOrder { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
