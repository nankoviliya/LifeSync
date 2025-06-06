﻿using LifeSync.API.Features.Finances.Models;
using LifeSync.API.Features.Finances.ResultMessages;
using LifeSync.API.Features.Finances.Services.Contracts;
using LifeSync.API.Models.Incomes;
using LifeSync.API.Models.Incomes.Events;
using LifeSync.API.Persistence;
using LifeSync.API.Shared;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Finances.Services;

public class IncomeTransactionsManagement : BaseService, IIncomeTransactionsManagement
{
    private readonly ApplicationDbContext _databaseContext;
    private readonly ILogger<IncomeTransactionsManagement> _logger;

    public IncomeTransactionsManagement(
        ApplicationDbContext databaseContext,
        ILogger<IncomeTransactionsManagement> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<DataResult<GetIncomeTransactionsResponse>> GetUserIncomesAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var userIdIsParsed = Guid.TryParse(userId, out Guid userIdGuid);

        if (!userIdIsParsed)
        {
            _logger.LogWarning("Invalid user id was provided: {UserId}, unable to parse", userId);

            return Failure<GetIncomeTransactionsResponse>(IncomeTrackingResultMessages.InvalidUserId);
        }

        var userIncomeTransactions = await _databaseContext.IncomeTransactions
            .Where(x => x.UserId == userIdGuid)
            .OrderByDescending(x => x.Date)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var userIncomeTransactionsDto = userIncomeTransactions.Select(x => new GetIncomeDto
        {
            Id = x.Id,
            Amount = x.Amount.Amount,
            Currency = x.Amount.Currency.Code,
            Date = x.Date.ToString("yyyy-MM-dd"),
            Description = x.Description
        }).ToList();

        var response = new GetIncomeTransactionsResponse
        {
            IncomeTransactions = userIncomeTransactionsDto
        };

        return Success(response);
    }

    public async Task<DataResult<Guid>> AddIncomeAsync(
        string userId,
        AddIncomeDto request,
        CancellationToken cancellationToken)
    {
        var userIdIsParsed = Guid.TryParse(userId, out Guid userIdGuid);

        if (!userIdIsParsed)
        {
            _logger.LogWarning("Invalid user id was provided: {UserId}, unable to parse", userId);

            return Failure<Guid>(IncomeTrackingResultMessages.InvalidUserId);
        }

        var incomeTransaction = new IncomeTransaction
        {
            Id = Guid.NewGuid(),
            Amount = new Money(request.Amount, Currency.FromCode(request.Currency)),
            Date = request.Date,
            Description = request.Description,
            UserId = userIdGuid
        };

        await _databaseContext.IncomeTransactions.AddAsync(incomeTransaction, cancellationToken);

        incomeTransaction.RaiseDomainEvent(new IncomeTransactionCreatedDomainEvent(userIdGuid, incomeTransaction));

        await _databaseContext.SaveChangesAsync(cancellationToken);

        return Success(incomeTransaction.Id);
    }
}
