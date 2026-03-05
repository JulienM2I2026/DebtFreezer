using Microsoft.EntityFrameworkCore;

namespace DebtService.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<String> Orders { get; set; }
    }
}
