using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.Expenses;

namespace LifeSync.API.Models.Expenses.Events;

public record ExpenseTransactionCreatedDomainEvent(Guid userId, ExpenseTransaction expenseTransaction) : IDomainEvent;