namespace ChallengeService.Dtos.External
{
    public class ExternalUserDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
