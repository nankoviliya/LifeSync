using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.Incomes;

namespace LifeSync.API.Models.Incomes.Events;

public record IncomeTransactionCreatedDomainEvent(Guid userId, IncomeTransaction incomeTransaction) : IDomainEvent;