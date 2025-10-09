using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeSync.API.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .HasMaxLength(500);

        builder.Property(x => x.LastName)
            .HasMaxLength(500);

        builder.Property(x => x.Email)
            .HasMaxLength(320);

        builder.HasOne(e => e.Language)
            .WithMany()
            .HasForeignKey(e => e.LanguageId)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.CurrencyPreference)
            .HasMaxLength(3)
            .IsRequired();

        builder.OwnsOne(x => x.Balance, balance =>
        {
            balance.Property(m => m.Amount).HasColumnName("Balance_Amount");
            balance.Property(m => m.CurrencyCode).HasColumnName("Balance_CurrencyCode").HasMaxLength(3);
        });
    }
}