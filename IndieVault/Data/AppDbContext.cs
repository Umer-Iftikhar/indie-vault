using IndieVault.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IndieVault.Data
{
    public class AppDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<ApplicationUser>
    {
        public  AppDbContext (DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Wishlist> Wishlists { get; set; } = null!;
        public DbSet<DownloadHistory> DownloadHistories { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<Screenshot> Screenshots { get; set; } = null!;

        // --- New 3NF Tables ---
        public DbSet<Engine> Engines { get; set; } = null!;
        public DbSet<Platform> Platforms { get; set; } = null!;
        public DbSet<GamePlatform> GamePlatforms { get; set; } = null!;
        public DbSet<GameTag> GameTags { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Composite key for GameTag
            builder.Entity<GameTag>().HasKey(gt => new { gt.GameId, gt.TagId });

            //Composite key for GamePlatform
            builder.Entity<GamePlatform>().HasKey(gp => new { gp.GameId, gp.PlatformId });

            // Unique constraints
            builder.Entity<Platform>().HasIndex(p => p.Name).IsUnique();
            builder.Entity<Genre>().HasIndex(g => g.Name).IsUnique();
            builder.Entity<Tag>().HasIndex(t => t.Name).IsUnique();
            builder.Entity<Engine>().HasIndex(e => e.Name).IsUnique();
            builder.Entity<ApplicationUser>().HasIndex(u => u.NormalizedEmail).IsUnique();
        }
    }
}
