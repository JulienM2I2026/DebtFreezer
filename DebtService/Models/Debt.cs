using DebtService.Enums;
using System.ComponentModel.DataAnnotations;

namespace DebtService.Models
{
    public class Debt
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Creditor name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Creditor name must be between 2 and 100 characters.")]
        public string Creditor { get; set; } = string.Empty;

        [Required(ErrorMessage = "Original amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public double OriginalAmount { get; set; }

        public double RemainingAmount { get; set; }

        [Required(ErrorMessage = "Interest rate is required.")]
        [Range(0, 100, ErrorMessage = "Interest rate must be between 0 and 100.")]
        public double InterestRate { get; set; }

        [Required(ErrorMessage = "Due date is required.")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Debt type is required.")]
        public int Type { get; set; }

        [Required(ErrorMessage = "Debt status is required.")]
        public int Status { get; set; }

        // Date du dernier paiement — utilisée pour calculer les intérêts accrués
        public DateTime? LastPaymentDate { get; set; }

    }
}
