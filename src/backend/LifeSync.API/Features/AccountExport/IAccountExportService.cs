
using LifeSync.API.Features.AccountExport.Exporters;
using LifeSync.API.Features.AccountExport.ResultMessages;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.AccountExport;

public interface IAccountExportService
{
    Task<DataResult<ExportAccountResponse>> ExportAccountData(
        string userId,
        ExportAccountRequest request,
        CancellationToken cancellationToken);
}

public class AccountExportService : BaseService, IAccountExportService
{
    private readonly ApplicationDbContext _databaseContext;
    private readonly IEnumerable<IAccountExporter> _exporters;
    private readonly ILogger<AccountExportService> _logger;

    public AccountExportService(
        ApplicationDbContext databaseContext,
        IEnumerable<IAccountExporter> exporters,
        ILogger<AccountExportService> logger)
    {
        _databaseContext = databaseContext;
        _exporters = exporters;
        _logger = logger;
    }

    public async Task<DataResult<ExportAccountResponse>> ExportAccountData(
        string userId,
        ExportAccountRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return Failure<ExportAccountResponse>(AccountExportResultMessages.InvalidUserId);
        }

        // TODO: fix relations and make it single query with joins
        var expenseTransactions = await _databaseContext.ExpenseTransactions
            .AsNoTracking()
            .Where(et => et.UserId == Guid.Parse(userId))
            .Select(et => new ExportAccountExpenseTransaction
            {
                Id = et.Id,
                Amount = et.Amount.Amount,
                Currency = et.Amount.Currency.Code,
                Description = et.Description,
                ExpenseType = et.ExpenseType,
                Date = et.Date,
            })
            .OrderBy(e => e.Date)
            .ToListAsync(cancellationToken);

        var incomeTransactions = await _databaseContext.IncomeTransactions.AsNoTracking()
            .Where(it => it.UserId == Guid.Parse(userId))
            .Select(it => new ExportAccountIncomeTransaction
            {
                Id = it.Id,
                Amount = it.Amount.Amount,
                Currency = it.Amount.Currency.Code,
                Description = it.Description,
                Date = it.Date,
            })
            .OrderBy(e => e.Date)
            .ToListAsync(cancellationToken);

        var accountData = await _databaseContext.Users
           .AsNoTracking()
           .Where(u => u.Id == userId)
           .Select(u => new ExportAccountData
           {
               ProfileData = new ExportAccountProfile
               {
                   UserId = u.Id,
                   UserName = u.UserName,
                   Email = u.Email,
                   FirstName = u.FirstName,
                   LastName = u.LastName,
                   BalanceAmount = u.Balance.Amount,
                   BalanceCurrency = u.Balance.Currency.Code,
                   LanguageId = u.Language.Id,
                   LanguageCode = u.Language.Code,
               },
               IncomeTransactions = incomeTransactions,
               ExpenseTransactions = expenseTransactions,
           })
           .FirstOrDefaultAsync(cancellationToken);

        if (accountData is null)
        {
            _logger.LogWarning("User not found for userId: {UserId}", userId);
            return Failure<ExportAccountResponse>(AccountExportResultMessages.UserNotFound);
        }

        var exporter = _exporters.Single(e => e.Format == request.Format);

        var result = await exporter.Export(accountData, cancellationToken);

        return Success(result);
    }
}
