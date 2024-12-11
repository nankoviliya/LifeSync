using PersonalFinances.API.Models.Abstractions;

namespace PersonalFinances.API.Models.Events;

public record IncomeTransactionCreatedDomainEvent(Guid userId, IncomeTransaction incomeTransaction) : IDomainEvent;