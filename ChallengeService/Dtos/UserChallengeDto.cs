namespace ChallengeService.Dtos
{
    public class UserChallengeDto
    {
        public int Id { get; set; }
        public int ChallengeId { get; set; }
        public Guid UserId { get; set; }
        public double AmountPaid { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
