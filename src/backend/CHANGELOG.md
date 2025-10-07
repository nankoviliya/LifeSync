# Changelog

## [Unreleased] - Finances Refactoring

### Breaking Changes

#### Endpoint Consolidation
The separate expense and income GET endpoints have been replaced with a unified search endpoint:

**Removed:**
- `GET /api/finances/transactions/expense` - Get expense transactions
- `GET /api/finances/transactions/income` - Get income transactions

**Replaced with:**
- `GET /api/finances/transactions` - Search all transactions with filters
  - Supports filtering by transaction type (Income, Expense)
  - Supports filtering by date range (StartDate, EndDate)
  - Supports filtering by description
  - Supports filtering by expense types (Needs, Wants, Savings)
  - Returns both expenses and incomes in a single response

**Migration Guide:**
- To get only expenses: Use `GET /api/finances/transactions?Filters.TransactionTypes=Expense`
- To get only incomes: Use `GET /api/finances/transactions?Filters.TransactionTypes=Income`
- To get both: Use `GET /api/finances/transactions?Filters.TransactionTypes=Expense&Filters.TransactionTypes=Income`

#### Error Response Format Changes
Error responses have changed from MVC controller format to FastEndpoints format:

**Before (MVC Controller):**
```json
{
  "errors": ["Error message 1", "Error message 2"]
}
```

**After (FastEndpoints):**
```json
{
  "errors": {
    "GeneralErrors": ["Error message 1", "Error message 2"]
  }
}
```

**Impact:** API consumers need to update their error handling logic to parse the new error response structure.

### Added
- Authorization (`JwtBearerDefaults.AuthenticationScheme`) now explicitly configured on all financial endpoints
- Explicit transaction rollback in all catch blocks for data consistency
- Comprehensive input validation on all requests

### Changed
- Migrated from ASP.NET Core MVC Controllers to FastEndpoints
- Improved error handling with explicit rollback on failures

### Security
- ✅ All financial endpoints now require Bearer token authentication
- ✅ Authorization is explicitly configured (not inherited)
