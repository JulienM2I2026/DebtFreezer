using AuthentificationService.Dtos;

namespace AuthentificationService.Services
{
    public interface IUserService
    {
        Task<AuthDto> RegisterAsync(RegisterDto dto);
        Task<AuthDto> LoginAsync(LoginDto dto);
        Task<UserProfileDto?> GetMeAsync(Guid userId);
        Task<string> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);
    }
}