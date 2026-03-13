namespace Gateway.Dtos
{
    // Ce que le frontend envoie pour créer un challenge (sans userId, injecté par le Gateway)
    public class CreateChallengeGatewayDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double TargetAmount { get; set; }
        public DateTime DueDate { get; set; }
    }

    // Ce que le Gateway envoie au ChallengeService (avec creatorUserId injecté depuis le token)
    public class CreateChallengeInternalDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double TargetAmount { get; set; }
        public DateTime DueDate { get; set; }
        public Guid CreatorUserId { get; set; }
    }

    // Réponse du ChallengeService pour un challenge
    public class ChallengeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double TargetAmount { get; set; }
        public double TotalPaid { get; set; }
        public double ProgressPercent { get; set; }
        public DateTime DueDate { get; set; }
        public int CreatorUserId { get; set; }
        public int ParticipantCount { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Progression collective d'un challenge
    public class ChallengeProgressDto
    {
        public int ChallengeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public double TargetAmount { get; set; }
        public double TotalPaid { get; set; }
        public double ProgressPercent { get; set; }
        public int ParticipantCount { get; set; }
        public int Status { get; set; }
        public DateTime DueDate { get; set; }
        public List<LeaderboardEntryDto> Leaderboard { get; set; } = new();
    }

    // Entrée du leaderboard
    public class LeaderboardEntryDto
    {
        public int Rank { get; set; }
        public int UserId { get; set; }
        public double AmountPaid { get; set; }
        public DateTime JoinedAt { get; set; }
    }

    // Pour rejoindre un challenge
    public class JoinChallengeGatewayDto
    {
        // UserId injecté depuis le token par le Gateway
    }

    // Pour enregistrer un paiement dans un challenge
    public class RecordChallengePaymentGatewayDto
    {
        public int PaymentId { get; set; }
    }
}
