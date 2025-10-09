# Migration Testing Guide

## Overview

This guide outlines the recommended approach for testing the `RemoveCurrenciesTable` migration before deploying to production. This migration restructures how currencies are stored in the database without losing existing user data.

## Migration Details

**Migration File**: `src/LifeSync.API/Persistence/Migrations/20251008203646_RemoveCurrenciesTable.cs`

**Purpose**: Simplify currency storage by replacing the `Currencies` table with a hardcoded `CurrencyRegistry`, and migrate currency data from old columns to new columns.

### What Changed

#### Before (Old Schema)
- `Currencies` table with dynamic currency data
- `Users.Balance_Currency` (nvarchar(max))
- `IncomeTransactions.Amount_Currency` (nvarchar(max))
- `ExpenseTransactions.Amount_Currency` (nvarchar(max))

#### After (New Schema)
- `CurrencyRegistry` static class (code-based, no table)
- `Users.Balance_CurrencyCode` (nvarchar(3))
- `IncomeTransactions.CurrencyCode` (nvarchar(3))
- `ExpenseTransactions.CurrencyCode` (nvarchar(3))

### Migration Steps

The migration executes in 8 carefully ordered steps:

1. **Add new columns** (temporarily nullable)
2. **Migrate data** from old currency columns to new columns
3. **Use fallback** (CurrencyPreference) for any NULL values
4. **Make columns non-nullable** after data is migrated
5. **Rename Amount columns** (flatten owned entity structure)
6. **Update CurrencyPreference** column constraints
7. **Drop old currency columns** (data already preserved)
8. **Drop Currencies table** (no longer needed)

## Testing Strategy

### Prerequisites

1. **Backup production database**
   ```bash
   # SQL Server backup command
   BACKUP DATABASE [LifeSyncDb]
   TO DISK = 'C:\Backups\LifeSyncDb_PreMigration_YYYYMMDD.bak'
   WITH COMPRESSION, CHECKSUM;
   ```

2. **Restore to test environment**
   ```bash
   RESTORE DATABASE [LifeSyncDb_Test]
   FROM DISK = 'C:\Backups\LifeSyncDb_PreMigration_YYYYMMDD.bak'
   WITH REPLACE, MOVE 'LifeSyncDb' TO 'C:\Test\LifeSyncDb_Test.mdf',
   MOVE 'LifeSyncDb_Log' TO 'C:\Test\LifeSyncDb_Test_Log.ldf';
   ```

### Pre-Migration Validation

Before running the migration, capture baseline data:

```sql
-- 1. Count of users
SELECT COUNT(*) AS TotalUsers FROM Users;

-- 2. Distribution of currency preferences
SELECT CurrencyPreference, COUNT(*) AS UserCount
FROM Users
GROUP BY CurrencyPreference
ORDER BY UserCount DESC;

-- 3. Check for any NULL or empty currencies
SELECT
    (SELECT COUNT(*) FROM Users WHERE Balance_Currency IS NULL OR Balance_Currency = '') AS UsersWithNullCurrency,
    (SELECT COUNT(*) FROM IncomeTransactions WHERE Amount_Currency IS NULL OR Amount_Currency = '') AS IncomesWithNullCurrency,
    (SELECT COUNT(*) FROM ExpenseTransactions WHERE Amount_Currency IS NULL OR Amount_Currency = '') AS ExpensesWithNullCurrency;

-- 4. Sample of current currency data
SELECT TOP 10
    u.Id,
    u.UserName,
    u.CurrencyPreference,
    u.Balance_Currency,
    COUNT(it.Id) AS IncomeCount,
    COUNT(et.Id) AS ExpenseCount
FROM Users u
LEFT JOIN IncomeTransactions it ON u.Id = it.UserId
LEFT JOIN ExpenseTransactions et ON u.Id = et.UserId
GROUP BY u.Id, u.UserName, u.CurrencyPreference, u.Balance_Currency;

-- 5. Check for unsupported currencies (not in registry)
SELECT DISTINCT Balance_Currency
FROM Users
WHERE Balance_Currency NOT IN ('USD', 'EUR', 'BGN', 'UAH', 'RUB')
  AND Balance_Currency IS NOT NULL
  AND Balance_Currency != '';
```

### Running the Migration

#### Option 1: Using EF Core CLI

```bash
# From the backend directory
cd src/backend
dotnet ef database update --project src/LifeSync.API --startup-project src/LifeSync.API
```

#### Option 2: Application Startup

The migration will automatically run when the application starts if auto-migration is enabled.

### Post-Migration Validation

After the migration completes, validate that data was preserved correctly:

```sql
-- 1. Verify user count unchanged
SELECT COUNT(*) AS TotalUsers FROM Users;

-- 2. Verify all users have currency codes
SELECT
    COUNT(*) AS TotalUsers,
    COUNT(CASE WHEN Balance_CurrencyCode IS NULL OR Balance_CurrencyCode = '' THEN 1 END) AS UsersWithoutCurrencyCode
FROM Users;

-- 3. Verify currency codes match preferences (for users that had no Balance_Currency)
SELECT
    CurrencyPreference,
    Balance_CurrencyCode,
    COUNT(*) AS Count
FROM Users
GROUP BY CurrencyPreference, Balance_CurrencyCode
ORDER BY Count DESC;

-- 4. Verify transaction currency codes
SELECT
    (SELECT COUNT(*) FROM IncomeTransactions WHERE CurrencyCode IS NULL OR CurrencyCode = '') AS IncomesWithNullCurrency,
    (SELECT COUNT(*) FROM ExpenseTransactions WHERE CurrencyCode IS NULL OR CurrencyCode = '') AS ExpensesWithNullCurrency;

-- 5. Verify old columns are gone
SELECT
    COLUMN_NAME,
    TABLE_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE COLUMN_NAME IN ('Balance_Currency', 'Amount_Currency');
-- Should return 0 rows

-- 6. Verify Currencies table is gone
SELECT
    TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'Currencies';
-- Should return 0 rows

-- 7. Sample comparison of users (compare with pre-migration data)
SELECT TOP 10
    u.Id,
    u.UserName,
    u.CurrencyPreference,
    u.Balance_CurrencyCode,
    COUNT(it.Id) AS IncomeCount,
    COUNT(et.Id) AS ExpenseCount
FROM Users u
LEFT JOIN IncomeTransactions it ON u.Id = it.UserId
LEFT JOIN ExpenseTransactions et ON u.Id = et.UserId
GROUP BY u.Id, u.UserName, u.CurrencyPreference, u.Balance_CurrencyCode;
```

### Functional Testing

After database validation, perform functional tests:

1. **User Login Test**
   - Log in with existing user accounts
   - Verify no authentication errors
   - Verify currency preference is displayed correctly

2. **Balance Display Test**
   - Check that user balances display with correct currency symbols
   - Verify amounts match pre-migration values

3. **Transaction Tests**
   - Create new income transaction
   - Create new expense transaction
   - Verify transactions are saved with correct currency codes
   - Verify transaction lists display correctly

4. **Currency Preference Change Test**
   - Change user's currency preference
   - Verify balance is converted using exchange rate
   - Verify new preference is saved correctly

5. **Data Export/Import Test**
   - Export user data
   - Verify exported JSON contains correct currency codes
   - Test importing data

## Rollback Strategy

If issues are discovered post-migration, use the backup to restore:

```sql
-- Stop the application first!
-- Then restore the database
RESTORE DATABASE [LifeSyncDb]
FROM DISK = 'C:\Backups\LifeSyncDb_PreMigration_YYYYMMDD.bak'
WITH REPLACE;
```

**Important**: You must also revert the application code to the previous version that expects the old schema.

## Common Issues and Solutions

### Issue 1: Unsupported Currency Codes

**Symptom**: Migration completes but some users have currency codes not in the registry (e.g., "GBP", "JPY")

**Solution**:
1. Identify affected users using the pre-migration query #5
2. Add missing currencies to `CurrencyRegistry.cs` before migration
3. Re-run migration

### Issue 2: Performance on Large Databases

**Symptom**: Migration takes too long or times out

**Solution**:
1. Increase command timeout in migration
2. Consider batch processing for very large datasets
3. Run during off-peak hours

### Issue 3: Foreign Key Constraints

**Symptom**: Migration fails due to foreign key violations

**Solution**: Verify all transactions reference valid users before migration:

```sql
-- Check for orphaned transactions
SELECT COUNT(*) FROM IncomeTransactions it
LEFT JOIN Users u ON it.UserId = u.Id
WHERE u.Id IS NULL;

SELECT COUNT(*) FROM ExpenseTransactions et
LEFT JOIN Users u ON et.UserId = u.Id
WHERE u.Id IS NULL;
```

## Production Deployment Checklist

- [ ] Database backup completed and verified
- [ ] Test environment restored from production backup
- [ ] Pre-migration validation queries executed and results documented
- [ ] Migration tested successfully in test environment
- [ ] Post-migration validation passed in test environment
- [ ] Functional tests passed in test environment
- [ ] No unsupported currency codes detected
- [ ] Rollback procedure tested
- [ ] Deployment window scheduled (low-traffic period)
- [ ] Support team notified
- [ ] Monitoring alerts configured
- [ ] Production migration executed
- [ ] Post-migration validation executed in production
- [ ] Functional smoke tests passed in production
- [ ] Application logs checked for errors

## Support Contacts

If issues arise during migration:

1. Check application logs for error details
2. Review migration execution logs
3. Consult this guide's troubleshooting section
4. Contact database administrator if rollback is needed

## Additional Notes

- **Downtime**: The migration requires application downtime to prevent data inconsistencies. Plan for 5-15 minutes depending on database size.
- **Data Loss Risk**: MITIGATED - The new migration preserves all existing currency data before dropping old columns.
- **Compatibility**: The rollback requires both database AND code to be reverted together.
