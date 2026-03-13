using System.ComponentModel.DataAnnotations;

namespace ChallengeService.Dtos
{
    public class JoinChallengeDto
    {
        [Required]
        public Guid UserId { get; set; }
    }
}
