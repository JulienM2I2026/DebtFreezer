using AuthentificationService.Data;
using AuthentificationService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthentificationService.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _db;

        public UserRepository(UserDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(User user)
        {
            await _db.users.AddAsync(user);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var normalized = email.Trim().ToLowerInvariant(); //variable qui contient une version “standardisée” de l'email
            return await _db.users.AnyAsync(u => u.Email.ToLower() == normalized);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var normalized = email.Trim().ToLowerInvariant();
            return await _db.users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalized);
        }

        public async Task<User?> GetByUserIdAsync(Guid userId)
        {
            return await _db.users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetByResetTokenAsync(string token)
        {
            return await _db.users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);
        }

        public Task UpdateAsync(User user)
        {
            _db.users.Update(user);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
