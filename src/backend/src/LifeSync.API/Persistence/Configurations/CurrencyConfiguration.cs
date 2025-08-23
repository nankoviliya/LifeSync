using LifeSync.API.Models.Currencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeSync.API.Persistence.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("Currencies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(3)
            .IsUnicode(false); // Typically, ISO codes are ASCII

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        // Currency symbol (e.g. $, €, £)
        builder.Property(c => c.Symbol)
            .IsRequired()
            .HasMaxLength(5);

        // Numeric code (e.g. 840 for USD)
        builder.Property(c => c.NumericCode)
            .IsRequired();

        builder.HasIndex(c => c.Code)
            .IsUnique();
    }
}
