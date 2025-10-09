using LifeSync.API.Features.AccountExport.DataExporters;
using LifeSync.API.Features.AccountExport.Models;
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
    private readonly IEnumerable<IAccountDataExporter> _dataExporters;
    private readonly ILogger<AccountExportService> _logger;

    public AccountExportService(
        ApplicationDbContext databaseContext,
        IEnumerable<IAccountDataExporter> dataExporters,
        ILogger<AccountExportService> logger)
    {
        _databaseContext = databaseContext;
        _dataExporters = dataExporters;
        _logger = logger;
    }

    public async Task<DataResult<ExportAccountResponse>> ExportAccountData(
        RequiredString userId,
        ExportAccountRequest request,
        CancellationToken cancellationToken)
    {
        IAccountDataExporter? dataExporter = _dataExporters.SingleOrDefault(e => e.Format == request.Format);

        if (dataExporter is null)
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
                    BalanceCurrency = u.Balance.CurrencyCode,
                    LanguageId = u.Language.Id,
                    LanguageCode = u.Language.Code
                },
                IncomeTransactions = u.IncomeTransactions
                    .Select(it => new ExportAccountIncomeTransaction
                    {
                        Id = it.Id,
                        Amount = it.Amount.Amount,
                        Currency = it.Amount.CurrencyCode,
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
                        Currency = et.Amount.CurrencyCode,
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

        ExportAccountResponse? result = await dataExporter.Export(accountData, cancellationToken);

        return Success(result);
    }
}