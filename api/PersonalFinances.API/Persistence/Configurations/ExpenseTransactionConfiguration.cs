using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinances.API.Models;
using PersonalFinances.API.Shared;

namespace PersonalFinances.API.Persistence.Configurations;

internal sealed class ExpenseTransactionConfiguration : IEntityTypeConfiguration<ExpenseTransaction>
{
    public void Configure(EntityTypeBuilder<ExpenseTransaction> builder)
    {
        builder.ToTable("ExpenseTransactions");

        builder.HasKey(x => x.Id);
        
        builder.OwnsOne(x => x.Amount, balance =>
        {
            balance.Property(price => price.Currency)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });
    }
}