using IndieVault.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndieVault.Api.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.Property(rt => rt.Token)
                .IsRequired();

            builder.Property(rt => rt.ExpiresAt)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(rt => rt.UserId);

            builder.Property(rt => rt.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(rt => rt.IsRevoked);

            builder.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
