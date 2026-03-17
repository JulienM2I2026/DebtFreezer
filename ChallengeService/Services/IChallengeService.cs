using ChallengeService.Dtos;

namespace ChallengeService.Services
{
    public interface IChallengeService
    {
        Task<ChallengeDto> CreateAsync(CreateChallengeDto dto);
        Task<List<ChallengeDto>> GetAllAsync();
        Task<ChallengeDto?> GetByIdAsync(int id);
        Task<UserChallengeDto> JoinAsync(int challengeId, JoinChallengeDto dto);
        Task<UserChallengeDto> RecordPaymentAsync(int challengeId, RecordChallengePaymentDto dto);
        Task<ChallengeProgressDto?> GetProgressAsync(int challengeId);
        Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(int challengeId);
        Task<bool> DeleteAsync(int challengeId);
    }
}
