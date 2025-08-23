using LifeSync.API.Features.AccountImport.Importers;
using LifeSync.API.Models.Expenses;
using LifeSync.API.Models.Incomes;
using LifeSync.API.Persistence;
using LifeSync.API.Shared;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;

namespace LifeSync.API.Features.AccountImport
{
    public interface IAccountImportService
    {
        Task<MessageResult> ImportAccountDataAsync(
            string userId,
            AccountImportRequest request,
            CancellationToken ct);
    }

    public class AccountImportService : BaseService, IAccountImportService
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IEnumerable<IAccountImporter> _importers;
        private readonly ILogger<AccountImportService> _logger;

        public AccountImportService(
            ApplicationDbContext databaseContext,
            IEnumerable<IAccountImporter> importers,
            ILogger<AccountImportService> logger)
        {
            _databaseContext = databaseContext;
            _importers = importers;
            _logger = logger;
        }

        public async Task<MessageResult> ImportAccountDataAsync(
            string userId,
            AccountImportRequest request,
            CancellationToken ct)
        {
            using var logScope = _logger.BeginScope("UserId:{UserId}, Format:{Format}", userId, request.Format);

            var importer = _importers.SingleOrDefault(i => i.Format == request.Format);
            if (importer is null)
            {
                _logger.LogWarning("Unsupported format");
                return FailureMessage("Unsupported file format.");
            }

            var data = await importer.ImportAsync(request.File, ct);
            if (data is null)
            {
                _logger.LogWarning("Import returned null");
                return MessageResult.Failure("Failed to parse file.");
            }

            var user = await _databaseContext.Users.FindAsync(new object[] { userId }, ct);
            if (user is null)
            {
                _logger.LogWarning("User not found");
                return MessageResult.Failure("User account not found.");
            }

            await using var tx = await _databaseContext.Database.BeginTransactionAsync(ct);
            try
            {
                if (data.ProfileData.BalanceAmount.HasValue && data.ProfileData.BalanceCurrency != null)
                {
                    user.Balance = new Money(data.ProfileData.BalanceAmount.Value,
                                              Currency.FromCode(data.ProfileData.BalanceCurrency));
                }
                user.LanguageId = data.ProfileData.LanguageId ?? user.LanguageId;
                
                _databaseContext.ExpenseTransactions.AddRange(data.ExpenseTransactions.Select(e => new ExpenseTransaction
                {
                    Amount = new Money(e.Amount, Currency.FromCode(e.Currency)),
                    Description = e.Description,
                    ExpenseType = e.ExpenseType,
                    Date = e.Date,
                    UserId = userId
                }));

                _databaseContext.IncomeTransactions.AddRange(data.IncomeTransactions.Select(i => new IncomeTransaction
                {
                    Amount = new Money(i.Amount, Currency.FromCode(i.Currency)),
                    Description = i.Description,
                    Date = i.Date,
                    UserId = userId
                }));

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
}
