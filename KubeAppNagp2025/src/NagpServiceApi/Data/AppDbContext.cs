using Microsoft.EntityFrameworkCore;
using NagpServiceApi.Models;

namespace NagpServiceApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "User One", Email = "userone@example.com" },
                new User { Id = 2, Name = "User Two", Email = "usertwo@example.com" },
                new User { Id = 3, Name = "User Three", Email = "userthree@example.com" },
                new User { Id = 4, Name = "User Four", Email = "userfour@example.com" },
                new User { Id = 5, Name = "User Five", Email = "userfive@example.com" }
            );
        }
    }
}
