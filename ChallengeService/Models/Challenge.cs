using ChallengeService.Enums;
using System.ComponentModel.DataAnnotations;

namespace ChallengeService.Models
{
    public class Challenge
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Target amount must be greater than 0.")]
        public double TargetAmount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public Guid CreatorUserId { get; set; }

        public ChallengeStatus Status { get; set; } = ChallengeStatus.ACTIVE;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation : participants du défi
        public ICollection<UserChallenge> Participants { get; set; } = new List<UserChallenge>();
    }
}
