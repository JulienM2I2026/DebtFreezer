using System.ComponentModel.DataAnnotations;

namespace ChallengeService.Dtos
{
    public class CreateChallengeDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Target amount must be greater than 0.")]
        public double TargetAmount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "CreatorUserId must be a valid positive integer.")]
        public int CreatorUserId { get; set; }
    }
}
