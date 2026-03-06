using AuthentificationService.Dtos;
using AuthentificationService.Models;
using AuthentificationService.Repository;
using AuthentificationService.Services;

namespace DebtService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtTokenService _jwt;

        public UserService(IUserRepository users, IPasswordHasher hasher, IJwtTokenService jwt)
        {
            _users = users;
            _hasher = hasher;
            _jwt = jwt;
        }

        public async Task<AuthDto> RegisterAsync(RegisterDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            var fullName = dto.FullName.Trim();

            // Vérifie unicité email
            var exists = await _users.ExistsByEmailAsync(email);
            if (exists)
                throw new InvalidOperationException("L'email saisi est déjà utilisé");

            // Crée user + hash password
            var user = new User(email, fullName, _hasher.Hash(dto.Password))
            {
                MonthlyRepaymentBudget = dto.MonthlyRepaymentBudget,
                RepaymentStrategy = dto.RepaymentStrategy
            };

            await _users.AddAsync(user);
            await _users.SaveChangesAsync();

            // Token
            var (token, exp) = _jwt.CreateAccessToken(user);

            return new AuthDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                AccessToken = token,
                ExpiresAtUtc = exp
            };
        }

        public async Task<AuthDto> LoginAsync(LoginDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            var user = await _users.GetByEmailAsync(email);
            if (user is null)
                throw new UnauthorizedAccessException("Le mot de passe ou l'email saisi est incorrect");

            if (!_hasher.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Le mot de passe ou l'email saisi est incorrect");

            var (token, exp) = _jwt.CreateAccessToken(user);

            return new AuthDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                AccessToken = token,
                ExpiresAtUtc = exp
            };
        }

        public async Task<UserProfileDto?> GetMeAsync(Guid userId)
        {
            var user = await _users.GetByUserIdAsync(userId);
            if (user is null) return null;

            return new UserProfileDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                MonthlyRepaymentBudget = user.MonthlyRepaymentBudget,
                RepaymentStrategy = user.RepaymentStrategy
            };
        }
    }
}