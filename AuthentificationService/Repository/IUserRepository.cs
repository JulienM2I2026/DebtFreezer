using AuthentificationService.Models;

namespace AuthentificationService.Repository
{
    public interface IUserRepository
    {
        Task<bool> ExistsByEmailAsync(string email);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUserIdAsync(Guid userId);

        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}
