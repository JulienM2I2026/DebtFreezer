using System.ComponentModel.DataAnnotations;

namespace Gateway.Dtos
{
    public class UpdatePaymentDto
    {
        [Range(0.01, double.MaxValue)]
        public decimal? Amount { get; set; }

        public DateTime? PaymentDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
