using BlogApi.Database.Configurations;
using BlogApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Database
{
    public class AppDbContext : DbContext{
        public AppDbContext()
        {
            
        }
        public AppDbContext(DbContextOptions options) : base(options)
        {
                
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityConfigurations).Assembly);     
        }
    }
}
