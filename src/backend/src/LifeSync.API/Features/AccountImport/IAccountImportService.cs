using FluentValidation;
using FluentValidation.Results;
using LifeSync.API.Features.AccountImport.DataReaders;
using LifeSync.API.Features.AccountImport.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Expenses;
using LifeSync.API.Models.Incomes;
using LifeSync.API.Models.Languages;
using LifeSync.API.Persistence;
using LifeSync.API.Shared;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.EntityFrameworkCore;
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
    private readonly IValidator<ImportAccountData> _validator;
    private readonly ILogger<AccountImportService> _logger;

    public AccountImportService(
        ApplicationDbContext databaseContext,
        IEnumerable<IAccountDataReader> dataReaders,
        IValidator<ImportAccountData> validator,
        ILogger<AccountImportService> logger)
    {
        _databaseContext = databaseContext;
        _dataReaders = dataReaders;
        _validator = validator;
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

        ValidationResult validationResult = await _validator.ValidateAsync(data, ct);
        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning("Invalid import data: {Errors}", errors);
            return MessageResult.Failure(errors);
        }

        User? user = await _databaseContext.Users.FindAsync(new object[] { userId.Value }, ct);
        if (user is null)
        {
            _logger.LogWarning("User not found, User Id: {UserId}", userId);
            return MessageResult.Failure("User account not found.");
        }

        string languageCode = data.ProfileData.LanguageCode.ToLower();

        Language? language = await _databaseContext.Languages
            .AsNoTracking()
            .FirstOrDefaultAsync(
                l => l.Code.ToLower().Equals(languageCode),
                ct);
        if (language is null)
        {
            _logger.LogWarning("Language not found, Language code: {LanguageCode}", languageCode);
            return MessageResult.Failure($"Language with code: {languageCode} not found");
        }

        await using IDbContextTransaction? tx = await _databaseContext.Database.BeginTransactionAsync(ct);
        try
        {
            user.UpdateBalance(
                new Money(data.ProfileData.BalanceAmount.ToRequiredStruct(),
                    data.ProfileData.BalanceCurrency.ToRequiredString())
            );

            user.UpdateLanguage(language.Id.ToRequiredStruct());

            IEnumerable<ExpenseTransaction> incomeTransactions = data.ExpenseTransactions.Select(e =>
                ExpenseTransaction.From(
                    new Money(e.Amount, e.Currency).ToRequiredReference(),
                    e.Date.ToRequiredStruct(),
                    e.Description.ToRequiredString(),
                    e.ExpenseType,
                    userId)
            ).ToList();

            IEnumerable<IncomeTransaction> expenseTransactions = data.IncomeTransactions.Select(i =>
                IncomeTransaction.From(
                    new Money(i.Amount, i.Currency).ToRequiredReference(),
                    i.Date.ToRequiredStruct(),
                    i.Description.ToRequiredString(),
                    userId)
            ).ToList();

            _databaseContext.Update(user);
            await _databaseContext.AddRangeAsync(incomeTransactions, ct);
            await _databaseContext.AddRangeAsync(expenseTransactions, ct);

            await _databaseContext.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return MessageResult.Success("Account data imported successfully.");
        }
        catch (ArgumentException ex) when (ex.ParamName == "currencyCode")
        {
            await tx.RollbackAsync(ct);
            _logger.LogWarning("Invalid currency in import. Error: {Error}", ex.Message);
            return MessageResult.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            _logger.LogError(ex, "Transaction failed");
            return MessageResult.Failure("Import failed. Please try again.");
        }
    }
}