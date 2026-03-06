using AuthentificationService.Models;
using System.ComponentModel.DataAnnotations;

namespace AuthentificationService.Dtos
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(120)]
        public string FullName { get; set; } = string.Empty;

        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;

        // Budget de remboursement
        [Range(0, 1_000_000)]
        public decimal MonthlyRepaymentBudget { get; set; } = 0m;

        // Stratégie de remboursement (Snowball/Avalanche) => fixé par défaut sur Snowball
        public StrategyType RepaymentStrategy { get; set; } = StrategyType.Snowball;
    }
}