using LifeSync.API.Models.Abstractions;

namespace LifeSync.API.Models.Events;

public record IncomeTransactionCreatedDomainEvent(Guid userId, IncomeTransaction incomeTransaction) : IDomainEvent;