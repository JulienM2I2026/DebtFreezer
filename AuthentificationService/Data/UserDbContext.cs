using AuthentificationService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthentificationService.Data
{
    public class UserDbContext : DbContext
    {

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> users { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionbuilder)
        //{
        //    string connectionString = "server=localhost;database=demo_efcore;user=root;password=1234test;";
        //    optionbuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        { 
            base.OnModelCreating(modelBuilder); 
            modelBuilder.Entity<User>(entity => 
                { 
                    entity.HasKey(x => x.UserId); 
                    entity.HasIndex(x => x.Email).IsUnique(); 
                    entity.Property(x => x.Email).IsRequired(); 
                    entity.Property(x => x.FullName).IsRequired(); 
                    entity.Property(x => x.PasswordHash).IsRequired(); 
            }); 
        }
    }

}
