using PersonalFinances.API.Models.Abstractions;

namespace PersonalFinances.API.Models.Events;

public record ExpenseTransactionCreatedDomainEvent(Guid userId, ExpenseTransaction expenseTransaction) : IDomainEvent;