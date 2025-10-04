using LifeSync.API.Features.AccountImport.DataReaders;
using LifeSync.API.Features.AccountImport.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Expenses;
using LifeSync.API.Models.Incomes;
using LifeSync.API.Persistence;
using LifeSync.API.Shared;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.EntityFrameworkCore.Storage;

namespace LifeSync.API.Features.AccountImport;

public interface IAccountImportService
{
    Task<MessageResult> ImportAccountDataAsync(
        RequiredString userId,
        AccountImportRequest request,
        CancellationToken ct);
}

public class AccountImportService : BaseService, IAccountImportService
{
    private readonly ApplicationDbContext _databaseContext;
    private readonly IEnumerable<IAccountDataReader> _dataReaders;
    private readonly ILogger<AccountImportService> _logger;

    public AccountImportService(
        ApplicationDbContext databaseContext,
        IEnumerable<IAccountDataReader> dataReaders,
        ILogger<AccountImportService> logger)
    {
        _databaseContext = databaseContext;
        _dataReaders = dataReaders;
        _logger = logger;
    }

    public async Task<MessageResult> ImportAccountDataAsync(
        RequiredString userId,
        AccountImportRequest request,
        CancellationToken ct)
    {
        using IDisposable? logScope = _logger.BeginScope("UserId:{UserId}, Format:{Format}", userId, request.Format);

        IAccountDataReader? dataReader = _dataReaders.SingleOrDefault(i => i.Format == request.Format);
        if (dataReader is null)
        {
            _logger.LogWarning("Unsupported format: {Format}", request.Format);
            return FailureMessage("Unsupported file format.");
        }

        ImportAccountData? data = await dataReader.ReadAsync(request.File, ct);
        if (data is null)
        {
            _logger.LogWarning("Cannot read data from file");
            return MessageResult.Failure("Cannot read data from file.");
        }

        User? user = await _databaseContext.Users.FindAsync(new object[] { userId }, ct);
        if (user is null)
        {
            _logger.LogWarning("User not found, User Id: {UserId}", userId);
            return MessageResult.Failure("User account not found.");
        }

        await using IDbContextTransaction? tx = await _databaseContext.Database.BeginTransactionAsync(ct);
        try
        {
            user.UpdateBalance(
                new Money(data.ProfileData.BalanceAmount.ToRequiredStruct(),
                    Currency.FromCode(data.ProfileData.BalanceCurrency.ToRequiredString()))
            );

            user.UpdateLanguage(data.ProfileData.LanguageId.ToRequiredStruct());

            IEnumerable<ExpenseTransaction> incomeTransactions = data.ExpenseTransactions.Select(e =>
                ExpenseTransaction.From(
                    new Money(e.Amount, Currency.FromCode(e.Currency)).ToRequiredReference(),
                    e.Date.ToRequiredStruct(),
                    e.Description.ToRequiredString(),
                    e.ExpenseType,
                    userId)
            ).ToList();

            IEnumerable<IncomeTransaction> expenseTransactions = data.IncomeTransactions.Select(i =>
                IncomeTransaction.From(
                    new Money(i.Amount, Currency.FromCode(i.Currency)).ToRequiredReference(),
                    i.Date.ToRequiredStruct(),
                    i.Description.ToRequiredString(),
                    userId)
            ).ToList();

            await _databaseContext.AddRangeAsync(incomeTransactions, ct);
            await _databaseContext.AddRangeAsync(expenseTransactions, ct);

            await _databaseContext.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return MessageResult.Success("Account data imported successfully.");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            _logger.LogError(ex, "Transaction failed");
            return MessageResult.Failure("Import failed. Please try again.");
        }
    }
}