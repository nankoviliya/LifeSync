using LifeSync.API.Features.AccountExport.Exporters;
using LifeSync.API.Features.AccountExport.ResultMessages;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.AccountExport;

public interface IAccountExportService
{
    Task<DataResult<ExportAccountResponse>> ExportAccountData(
        RequiredString userId,
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
        RequiredString userId,
        ExportAccountRequest request,
        CancellationToken cancellationToken)
    {
        IAccountExporter? exporter = _exporters.SingleOrDefault(e => e.Format == request.Format);

        if (exporter is null)
        {
            return Failure<ExportAccountResponse>(AccountExportResultMessages.ExportAccountDataFileFormatInvalid);
        }

        ExportAccountData? accountData = await _databaseContext.Users
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
                    LanguageCode = u.Language.Code
                },
                IncomeTransactions = u.IncomeTransactions
                    .Select(it => new ExportAccountIncomeTransaction
                    {
                        Id = it.Id,
                        Amount = it.Amount.Amount,
                        Currency = it.Amount.Currency.Code,
                        Description = it.Description,
                        Date = it.Date
                    })
                    .OrderBy(it => it.Date)
                    .ToList(),
                ExpenseTransactions = u.ExpenseTransactions
                    .Select(et => new ExportAccountExpenseTransaction
                    {
                        Id = et.Id,
                        Amount = et.Amount.Amount,
                        Currency = et.Amount.Currency.Code,
                        Description = et.Description,
                        ExpenseType = et.ExpenseType,
                        Date = et.Date
                    })
                    .OrderBy(et => et.Date)
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (accountData is null)
        {
            _logger.LogWarning("User not found for userId: {UserId}", userId);
            return Failure<ExportAccountResponse>(AccountExportResultMessages.UserNotFound);
        }

        ExportAccountResponse? result = await exporter.Export(accountData, cancellationToken);

        return Success(result);
    }
}