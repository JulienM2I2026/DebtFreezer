namespace StrategyService.Dtos.External
{
    /// <summary>
    /// Représentation d'une dette récupérée depuis DebtService.
    /// Correspond au DebtSend DTO de DebtService.
    /// </summary>
    public class ExternalDebtDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Creditor { get; set; } = string.Empty;
        public double OriginalAmount { get; set; }
        public double RemainingAmount { get; set; }
        public double InterestRate { get; set; }
        public DateTime DueDate { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
    }
}
