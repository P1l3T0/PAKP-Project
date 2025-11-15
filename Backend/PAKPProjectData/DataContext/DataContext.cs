using Microsoft.EntityFrameworkCore;

namespace PAKPProjectData
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>()
                .HasOne(n => n.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(n => n.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}