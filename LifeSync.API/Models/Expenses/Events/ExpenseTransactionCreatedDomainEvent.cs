using LifeSync.API.Models.Abstractions;

namespace LifeSync.API.Models.Expenses.Events;

public record ExpenseTransactionCreatedDomainEvent(Guid userId, ExpenseTransaction expenseTransaction) : IDomainEvent;