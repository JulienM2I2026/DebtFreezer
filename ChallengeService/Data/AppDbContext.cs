using ChallengeService.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengeService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<UserChallenge> UserChallenges { get; set; }
        public DbSet<ChallengePayment> ChallengePayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Index unique : un user ne peut rejoindre le même défi qu'une seule fois
            modelBuilder.Entity<UserChallenge>()
                .HasIndex(uc => new { uc.ChallengeId, uc.UserId })
                .IsUnique();

            modelBuilder.Entity<UserChallenge>()
                .HasIndex(uc => uc.UserId);

            modelBuilder.Entity<Challenge>()
                .HasIndex(c => c.CreatorUserId);

            // Index unique : un paiement PaymentService ne peut être compté
            // qu'une seule fois par défi (anti double-comptage)
            modelBuilder.Entity<ChallengePayment>()
                .HasIndex(cp => new { cp.ChallengeId, cp.PaymentId })
                .IsUnique();

            modelBuilder.Entity<ChallengePayment>()
                .HasIndex(cp => new { cp.ChallengeId, cp.UserId });
        }
    }
}
