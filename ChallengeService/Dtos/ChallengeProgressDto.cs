using ChallengeService.Enums;

namespace ChallengeService.Dtos
{
    public class ChallengeProgressDto
    {
        public int ChallengeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public double TargetAmount { get; set; }
        public double TotalPaid { get; set; }
        public double ProgressPercent { get; set; }
        public int ParticipantCount { get; set; }
        public ChallengeStatus Status { get; set; }
        public DateTime DueDate { get; set; }
        public List<LeaderboardEntryDto> Leaderboard { get; set; } = new();
    }
}
