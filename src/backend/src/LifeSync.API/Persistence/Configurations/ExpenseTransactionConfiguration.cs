using LifeSync.API.Models.Expenses;
using LifeSync.API.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeSync.API.Persistence.Configurations;

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

        // Soft delete query filter
        builder.HasQueryFilter(x => !x.IsDeleted);

        // Index for soft delete queries
        builder.HasIndex(x => x.IsDeleted);

        // Index for audit queries - CreatedAt is frequently used for filtering/sorting transactions
        builder.HasIndex(x => x.CreatedAt);

        // Composite index for common query pattern: active transactions by creation date
        builder.HasIndex(x => new { x.IsDeleted, x.CreatedAt });
    }
}