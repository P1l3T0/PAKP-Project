using Microsoft.EntityFrameworkCore;

namespace PAKPProjectData
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserPost> UserPosts { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>()
                .HasOne(n => n.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(n => n.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPost>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserProfile>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<UserProfile>(p => p.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}