using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LifeSync.API.Shared;
using LifeSync.API.Models.Incomes;

namespace LifeSync.API.Persistence.Configurations;

internal sealed class IncomeTransactionConfiguration : IEntityTypeConfiguration<IncomeTransaction>
{
    public void Configure(EntityTypeBuilder<IncomeTransaction> builder)
    {
        builder.ToTable("IncomeTransactions");

        builder.HasKey(x => x.Id);
        
        builder.OwnsOne(x => x.Amount, balance =>
        {
            balance.Property(price => price.Currency)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });
    }
}