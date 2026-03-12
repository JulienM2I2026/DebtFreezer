namespace StrategyService.Dtos.External
{
    /// <summary>
    /// Représentation d'une dette récupérée depuis DebtService.
    /// Correspond au DebtSend DTO de DebtService.
    /// </summary>
    public class ExternalDebtDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Creditor { get; set; } = string.Empty;
        public double OriginalAmount { get; set; }
        public double InterestRate { get; set; }
        public DateTime DueDate { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
