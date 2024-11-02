using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinances.API.Models;
using PersonalFinances.API.Shared;

namespace PersonalFinances.API.Persistence.Configurations;

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

        builder.HasIndex(x => x.Email)
            .IsUnique();
        
        builder.OwnsOne(x => x.Balance, balance =>
        {
            balance.Property(price => price.Currency)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });
        
        builder.OwnsOne(e => e.CurrencyPreference, currency =>
        {
            currency.Property(c => c.Code).HasColumnName("CurrencyPreference");
        });
    }
}