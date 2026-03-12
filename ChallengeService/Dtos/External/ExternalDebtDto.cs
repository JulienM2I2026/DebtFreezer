namespace ChallengeService.Dtos.External
{
    /// <summary>
    /// Représentation d'une dette récupérée depuis DebtService.
    /// Utilisée pour valider que l'UserId est propriétaire d'une dette.
    /// </summary>
    public class ExternalDebtDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Creditor { get; set; } = string.Empty;
        public double OriginalAmount { get; set; }
        public double InterestRate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
