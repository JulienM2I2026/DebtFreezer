namespace StrategyService.Dtos.External
{
    /// <summary>
    /// Représentation d'un paiement récupéré depuis PaymentService.
    /// Correspond au PaymentDto de PaymentService.
    /// </summary>
    public class ExternalPaymentDto
    {
        public int Id { get; set; }
        public int DebtId { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }
    }
}
