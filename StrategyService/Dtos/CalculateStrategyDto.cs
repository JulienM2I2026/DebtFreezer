using StrategyService.Enums;
using System.ComponentModel.DataAnnotations;

namespace StrategyService.Dtos
{
    /// <summary>
    /// Requête de calcul de plan de remboursement.
    /// Les dettes sont récupérées automatiquement depuis DebtService
    /// et les montants restants depuis PaymentService.
    /// </summary>
    public class CalculateStrategyDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "UserId must be a valid positive integer.")]
        public int UserId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Monthly budget must be greater than 0.")]
        public double MonthlyBudget { get; set; }

        [Required]
        [EnumDataType(typeof(StrategyType))]
        public StrategyType StrategyType { get; set; }
    }
}
