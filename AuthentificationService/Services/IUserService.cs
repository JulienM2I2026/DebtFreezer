using AuthentificationService.Dtos;

namespace AuthentificationService.Services
{
    public interface IUserService
    {
        Task<AuthDto> RegisterAsync(RegisterDto dto);
        Task<AuthDto> LoginAsync(LoginDto dto);
        Task<UserProfileDto?> GetMeAsync(Guid userId);
        Task<ForgotPasswordResultDto> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);
        Task<List<UserSummaryDto>> GetAllUsersAsync();
    }
}
