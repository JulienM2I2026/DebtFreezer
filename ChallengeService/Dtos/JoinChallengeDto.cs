using System.ComponentModel.DataAnnotations;

namespace ChallengeService.Dtos
{
    public class JoinChallengeDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "UserId must be a valid positive integer.")]
        public int UserId { get; set; }
    }
}
