using IndieVault.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndieVault.Api.Data.Configurations
{
    public class ScreenshotConfiguration : IEntityTypeConfiguration<Screenshot>
    {
        public void Configure(EntityTypeBuilder<Screenshot> builder)
        {
            builder.ToTable("Screenshots");

            builder.Property(s => s.ImagePath)
                .IsRequired()
                .HasMaxLength(500);
        }
    }
}
