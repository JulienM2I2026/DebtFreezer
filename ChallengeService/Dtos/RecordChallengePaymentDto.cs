using System.ComponentModel.DataAnnotations;

namespace ChallengeService.Dtos
{
    /// <summary>
    /// Enregistre un paiement existant de PaymentService dans un défi.
    /// Le montant est automatiquement lu depuis PaymentService.
    /// </summary>
    public class RecordChallengePaymentDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "UserId must be a valid positive integer.")]
        public int UserId { get; set; }

        /// <summary>ID du paiement dans PaymentService.</summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PaymentId must be a valid positive integer.")]
        public int PaymentId { get; set; }
    }
}
