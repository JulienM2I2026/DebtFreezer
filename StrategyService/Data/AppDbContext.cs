using Microsoft.EntityFrameworkCore;
using StrategyService.Models;

namespace StrategyService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<RepaymentPlan> RepaymentPlans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RepaymentPlan>(entity =>
            {
                // Requêtes fréquentes par utilisateur
                entity.HasIndex(p => p.UserId);
                // Tri par date de création
                entity.HasIndex(p => p.CreatedAt);
            });
        }
    }
}
