using LifeSync.API.Models.RefreshTokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeSync.API.Persistence.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.DeviceType)
            .IsRequired();

        builder.Property(x => x.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.RevokedAt);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Composite index for token lookup
        builder.HasIndex(x => new { x.TokenHash, x.UserId })
            .HasDatabaseName("IX_RefreshTokens_TokenHash_UserId");

        // Index for cleanup job
        builder.HasIndex(x => new { x.ExpiresAt, x.IsRevoked })
            .HasDatabaseName("IX_RefreshTokens_ExpiresAt_IsRevoked");

        // Index for user token management
        builder.HasIndex(x => new { x.UserId, x.DeviceType })
            .HasDatabaseName("IX_RefreshTokens_UserId_DeviceType");
    }
}