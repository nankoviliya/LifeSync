using Microsoft.EntityFrameworkCore;
using PersonalFinances.API.Features.IncomeTracking.Models;
using PersonalFinances.API.Models;
using PersonalFinances.API.Persistence;

namespace PersonalFinances.API.Features.IncomeTracking.Services;

public class IncomeTrackingService : IIncomeTrackingService
{
    private readonly ApplicationDbContext databaseContext;

    public IncomeTrackingService(ApplicationDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<IEnumerable<GetIncomeDto>> GetUserIncomesAsync(Guid userId)
    {
        var userIncomeTransactions = await databaseContext.IncomeTransactions
            .Where(x => x.UserId == userId)
            .AsNoTracking()
            .ToListAsync();

        var userIncomeTransactionsDto = userIncomeTransactions.Select(x => new GetIncomeDto
        {
            Id = x.Id,
            Amount = x.Amount,
            Date = x.Date,
            Description = x.Description
        });
        
        return userIncomeTransactionsDto;
    }

    public async Task<Guid> AddIncomeAsync(Guid userId, AddIncomeDto request)
    {
        var incomeTransaction = new IncomeTransaction
        {
            Id = Guid.NewGuid(),
            Amount = request.Amount,
            Date = request.Date,
            Description = request.Description,
            UserId = userId
        };
        
        await databaseContext.IncomeTransactions.AddAsync(incomeTransaction);
        await databaseContext.SaveChangesAsync();

        return incomeTransaction.Id;
    }
}