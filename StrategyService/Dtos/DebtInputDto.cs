using System.ComponentModel.DataAnnotations;

namespace StrategyService.Dtos
{
    public class DebtInputDto
    {
        [Required(ErrorMessage = "Creditor name is required.")]
        [StringLength(100, MinimumLength = 2)]
        public string Creditor { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Remaining amount must be greater than 0.")]
        public double RemainingAmount { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Interest rate must be between 0 and 100.")]
        public double InterestRate { get; set; }
    }
}
