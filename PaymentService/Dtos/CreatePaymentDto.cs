using System.ComponentModel.DataAnnotations;

namespace PaymentService.Dtos;

public class CreatePaymentDto
{
    [Required]
    public int DebtId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
