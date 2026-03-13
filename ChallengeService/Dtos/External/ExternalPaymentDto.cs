namespace ChallengeService.Dtos.External
{
    /// <summary>
    /// Représentation d'un paiement récupéré depuis PaymentService.
    /// </summary>
    public class ExternalPaymentDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }
        public ExternalDebtDto? Debt { get; set; }
    }
}
