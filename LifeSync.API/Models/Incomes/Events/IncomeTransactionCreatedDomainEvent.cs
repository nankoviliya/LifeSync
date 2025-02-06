using LifeSync.API.Models.Abstractions;

namespace LifeSync.API.Models.Incomes.Events;

public record IncomeTransactionCreatedDomainEvent(Guid userId, IncomeTransaction incomeTransaction) : IDomainEvent;