using DebtService.Models;
using Microsoft.EntityFrameworkCore;

namespace DebtService.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Debt> Debts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Debt>(entity =>
            {
                // Requêtes fréquentes par utilisateur
                entity.HasIndex(d => d.UserId);
                // Filtrage par statut (actif/remboursé)
                entity.HasIndex(d => d.Status);
                // Filtrage combiné userId + status
                entity.HasIndex(d => new { d.UserId, d.Status });
            });
        }
    }
}
