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

        builder.Property(x => x.IsRevoked)
            .IsRequired();

        builder.Property(x => x.RevokedAt);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.Property(x => x.DeletedAt);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.TokenHash })
            .HasDatabaseName("IX_RefreshTokens_UserId_TokenHash");

        builder.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("IX_RefreshTokens_ExpiresAt");

        builder.HasIndex(x => x.IsRevoked)
            .HasDatabaseName("IX_RefreshTokens_IsRevoked");
    }
}
