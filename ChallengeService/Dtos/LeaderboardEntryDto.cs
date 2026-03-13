namespace ChallengeService.Dtos
{
    public class LeaderboardEntryDto
    {
        public int Rank { get; set; }
        public Guid UserId { get; set; }
        public double AmountPaid { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
