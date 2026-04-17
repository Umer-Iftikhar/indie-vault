using IndieGamePlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IndieGamePlatform.Data
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
        public DbSet<GameTag> GameTags { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<Screenshot> Screenshots { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<GameTag>().HasKey(gt => new { gt.GameId, gt.TagId });
        }
    }
}
