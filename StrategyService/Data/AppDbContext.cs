using Microsoft.EntityFrameworkCore;
using StrategyService.Models;

namespace StrategyService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<RepaymentPlan> RepaymentPlans { get; set; }
    }
}
