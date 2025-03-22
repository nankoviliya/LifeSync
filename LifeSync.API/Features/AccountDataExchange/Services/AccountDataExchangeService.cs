using LifeSync.API.Features.AccountDataExchange.Models;
using LifeSync.API.Features.AccountDataExchange.ResultMessages;
using LifeSync.API.Features.AccountDataExchange.Services.ExportData;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.AccountDataExchange.Services
{
    public class AccountDataExchangeService : BaseService, IAccountDataExchangeService
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IAccountDataFileExporter _accountDataFileExporter;
        private readonly ILogger<AccountDataExchangeService> _logger;

        public AccountDataExchangeService(
            ApplicationDbContext databaseContext,
            IAccountDataFileExporter accountDataFileExport,
            ILogger<AccountDataExchangeService> logger)
        {
            _databaseContext = databaseContext;
            _accountDataFileExporter = accountDataFileExport;
            _logger = logger;
        }

        public async Task<DataResult<ExportAccountFileResultDto>> ExportAccountData(string userId, ExportAccountFileFormat fileFormat)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Failure<ExportAccountFileResultDto>(AccountDataExchangeResultMessages.InvalidUserId);
            }

            // TODO: fix relations and make it single query with joins
            var expenseTransactions = await _databaseContext.ExpenseTransactions
                .AsNoTracking()
                .Where(et => et.UserId == Guid.Parse(userId))
                .Select(et => new ExportAccountExpenseTransactionDataDto
                {
                    Id = et.Id,
                    Amount = et.Amount.Amount,
                    Currency = et.Amount.Currency.Code,
                    Description = et.Description,
                    ExpenseType = et.ExpenseType,
                })
                .ToListAsync();

            var incomeTransactions = await _databaseContext.IncomeTransactions.AsNoTracking()
                .Where(it => it.UserId == Guid.Parse(userId))
                .Select(it => new ExportAccountIncomeTransactionDataDto
                {
                    Id = it.Id,
                    Amount = it.Amount.Amount,
                    Currency = it.Amount.Currency.Code,
                    Description = it.Description,
                })
                .ToListAsync();

            var accountData = await _databaseContext.Users
               .AsNoTracking()
               .Where(u => u.Id == userId)
               .Select(u => new ExportAccountDataDto
               {
                   ProfileData = new ExportAccountProfileDataDto
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
               .FirstOrDefaultAsync();

            if (accountData is null)
            {
                _logger.LogWarning("User not found for userId: {UserId}", userId);
                return Failure<ExportAccountFileResultDto>(AccountDataExchangeResultMessages.UserNotFound);
            }

            var exportResult = await _accountDataFileExporter.ExportAccountData(fileFormat, accountData);

            return exportResult;
        }
    }
}
