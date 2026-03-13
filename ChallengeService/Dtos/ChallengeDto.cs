using ChallengeService.Enums;

namespace ChallengeService.Dtos
{
    public class ChallengeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double TargetAmount { get; set; }
        public double TotalPaid { get; set; }
        public double ProgressPercent { get; set; }
        public DateTime DueDate { get; set; }
        public Guid CreatorUserId { get; set; }
        public int ParticipantCount { get; set; }
        public ChallengeStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
