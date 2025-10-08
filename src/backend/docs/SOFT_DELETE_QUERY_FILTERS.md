# Soft Delete Query Filters

## Overview

This application implements **soft delete** functionality for the following entities:
- `Currency`
- `Language`
- `ExpenseTransaction`
- `IncomeTransaction`

Soft delete means records are not physically removed from the database. Instead, they are marked as deleted using the `IsDeleted` flag and `DeletedAt` timestamp.

## How Query Filters Work

Entity Framework Core automatically applies **global query filters** to hide soft-deleted records from all queries. This is configured in the entity configurations:

```csharp
// Example from CurrencyConfiguration.cs
builder.HasQueryFilter(c => !c.IsDeleted);
```

### Default Behavior

By default, **all queries automatically exclude soft-deleted records**:

```csharp
// This query will NOT return deleted currencies
var currencies = await dbContext.Currencies.ToListAsync();

// This will return null if the currency is soft-deleted
var currency = await dbContext.Currencies.FindAsync(currencyId);

// Count will not include deleted records
var count = await dbContext.Currencies.CountAsync();
```

## Accessing Soft-Deleted Records

### Using IgnoreQueryFilters()

To include soft-deleted records in your queries, use `IgnoreQueryFilters()`:

```csharp
// Get ALL currencies, including soft-deleted ones
var allCurrencies = await dbContext.Currencies
    .IgnoreQueryFilters()
    .ToListAsync();

// Get only soft-deleted currencies
var deletedCurrencies = await dbContext.Currencies
    .IgnoreQueryFilters()
    .Where(c => c.IsDeleted)
    .ToListAsync();

// Get a specific currency even if it's deleted
var currency = await dbContext.Currencies
    .IgnoreQueryFilters()
    .FirstOrDefaultAsync(c => c.Id == currencyId);
```

### Repository Pattern Example

If using repositories, create methods that explicitly handle soft-deleted entities:

```csharp
public class CurrencyRepository
{
    private readonly ApplicationDbContext _context;

    // Gets only active (non-deleted) currencies
    public async Task<List<Currency>> GetAllAsync()
    {
        return await _context.Currencies.ToListAsync();
    }

    // Gets all currencies including deleted ones
    public async Task<List<Currency>> GetAllIncludingDeletedAsync()
    {
        return await _context.Currencies
            .IgnoreQueryFilters()
            .ToListAsync();
    }

    // Gets only soft-deleted currencies
    public async Task<List<Currency>> GetDeletedAsync()
    {
        return await _context.Currencies
            .IgnoreQueryFilters()
            .Where(c => c.IsDeleted)
            .ToListAsync();
    }

    // Finds by ID, including deleted
    public async Task<Currency?> FindByIdIncludingDeletedAsync(Guid id)
    {
        return await _context.Currencies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}
```

## Soft Delete Operations

### Marking an Entity as Deleted

Use the `MarkAsDeleted()` method on the entity:

```csharp
var currency = await dbContext.Currencies.FindAsync(currencyId);
if (currency != null)
{
    currency.MarkAsDeleted();
    await dbContext.SaveChangesAsync();
}
```

### Restoring a Soft-Deleted Entity

Use the `Restore()` method:

```csharp
var currency = await dbContext.Currencies
    .IgnoreQueryFilters()
    .FirstOrDefaultAsync(c => c.Id == currencyId);

if (currency != null && currency.IsDeleted)
{
    currency.Restore();
    await dbContext.SaveChangesAsync();
}
```

## Important Considerations

### 1. Navigation Properties

Query filters do **NOT** cascade to related entities by default. If you load a User with their ExpenseTransactions, you need to explicitly handle the filter:

```csharp
// This will NOT include deleted transactions automatically
var user = await dbContext.Users
    .Include(u => u.ExpenseTransactions)
    .FirstOrDefaultAsync(u => u.Id == userId);

// To include deleted transactions, use IgnoreQueryFilters on the navigation
var userWithAllTransactions = await dbContext.Users
    .Include(u => u.ExpenseTransactions)
    .IgnoreQueryFilters()
    .FirstOrDefaultAsync(u => u.Id == userId);

// Or filter manually
var userWithDeletedTransactions = await dbContext.Users
    .Include(u => u.ExpenseTransactions.Where(t => t.IsDeleted))
    .FirstOrDefaultAsync(u => u.Id == userId);
```

### 2. Foreign Key Constraints

Soft-deleted entities still exist in the database, so foreign key relationships remain intact. This can cause issues:

```csharp
// If a Currency is soft-deleted but Users still reference it,
// you may need to handle this in your application logic

// Example: Check if referenced currency is deleted
var user = await dbContext.Users.FindAsync(userId);
var currency = await dbContext.Currencies
    .IgnoreQueryFilters()
    .FirstOrDefaultAsync(c => c.Id == user.CurrencyPreference.Id);

if (currency?.IsDeleted == true)
{
    // Handle the case where the user's preferred currency is deleted
    // Maybe assign a default currency or prompt user to select a new one
}
```

### 3. Unique Constraints

Soft-deleted records still occupy unique constraint slots. Consider this when implementing business logic:

```csharp
// Currency.Code has a unique index
// If you soft-delete USD and try to create a new USD, it will fail

// Solution 1: Restore instead of creating new
var existingCurrency = await dbContext.Currencies
    .IgnoreQueryFilters()
    .FirstOrDefaultAsync(c => c.Code == "USD");

if (existingCurrency != null)
{
    if (existingCurrency.IsDeleted)
    {
        existingCurrency.Restore();
        // Update other properties if needed
    }
    return existingCurrency;
}

// Solution 2: Modify unique constraint to include IsDeleted flag
// (requires database migration)
```

### 4. Performance Considerations

Query filters add a `WHERE IsDeleted = 0` clause to every query. This is efficient if you have an index on `IsDeleted` (which this application has):

```csharp
// Entity configurations include:
builder.HasIndex(c => c.IsDeleted);
```

For reporting or auditing queries that frequently access deleted records, consider:
- Using `IgnoreQueryFilters()` and filtering manually
- Creating separate views or read models for admin/audit purposes

### 5. Testing

When writing tests, be aware of query filters:

```csharp
[Fact]
public async Task SoftDeletedCurrencies_ShouldNotBeReturned()
{
    // Arrange
    var currency = Currency.From("USD", "US Dollar", "Dollar", "$", 840);
    dbContext.Currencies.Add(currency);
    await dbContext.SaveChangesAsync();

    currency.MarkAsDeleted();
    await dbContext.SaveChangesAsync();

    // Act
    var result = await dbContext.Currencies.ToListAsync();

    // Assert
    result.Should().NotContain(c => c.Id == currency.Id);
}

[Fact]
public async Task IgnoreQueryFilters_ShouldReturnDeletedCurrencies()
{
    // Arrange
    var currency = Currency.From("EUR", "Euro", "Euro", "€", 978);
    dbContext.Currencies.Add(currency);
    await dbContext.SaveChangesAsync();

    currency.MarkAsDeleted();
    await dbContext.SaveChangesAsync();

    // Act
    var result = await dbContext.Currencies
        .IgnoreQueryFilters()
        .Where(c => c.IsDeleted)
        .ToListAsync();

    // Assert
    result.Should().Contain(c => c.Id == currency.Id);
}
```

## Summary

- **Default**: Soft-deleted records are hidden automatically
- **To access deleted records**: Use `.IgnoreQueryFilters()`
- **To soft delete**: Call `entity.MarkAsDeleted()` and save
- **To restore**: Call `entity.Restore()` and save
- **Watch out for**: Navigation properties, foreign keys, unique constraints
- **Performance**: Indexes on `IsDeleted` are in place

## References

- Entity Framework Core Query Filters: https://learn.microsoft.com/en-us/ef/core/querying/filters
- Soft Delete Pattern: https://learn.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types#soft-delete
