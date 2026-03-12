using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChallengeService.Models
{
    public class UserChallenge
    {
        public int Id { get; set; }

        [Required]
        public int ChallengeId { get; set; }

        [ForeignKey(nameof(ChallengeId))]
        public Challenge? Challenge { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        [Range(0, double.MaxValue)]
        public double AmountPaid { get; set; } = 0;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
