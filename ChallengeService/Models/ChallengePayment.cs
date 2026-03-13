using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChallengeService.Models
{
    /// <summary>
    /// Enregistre chaque paiement PaymentService référencé dans un défi.
    /// L'index unique (ChallengeId, PaymentId) empêche le double-comptage.
    /// </summary>
    public class ChallengePayment
    {
        public int Id { get; set; }

        [Required]
        public int ChallengeId { get; set; }

        [ForeignKey(nameof(ChallengeId))]
        public Challenge? Challenge { get; set; }

        [Required]
        public Guid UserId { get; set; }

        /// <summary>ID du paiement dans PaymentService</summary>
        [Required]
        public int PaymentId { get; set; }

        [Range(0.01, double.MaxValue)]
        public double Amount { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}
