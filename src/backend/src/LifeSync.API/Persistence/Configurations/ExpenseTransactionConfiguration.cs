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

        // Auditing columns with SQL default values
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Soft delete query filter
        builder.HasQueryFilter(x => !x.IsDeleted);

        // Index for soft delete queries
        builder.HasIndex(x => x.IsDeleted);
    }
}