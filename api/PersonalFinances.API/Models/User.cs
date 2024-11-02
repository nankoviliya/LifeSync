using PersonalFinances.API.Shared;

namespace PersonalFinances.API.Models;

public class User
{
    public Guid Id { get; init; } // Primary Key
    
    public string Username { get; init; } = default!;
    
    public string FirstName { get; init; } = default!;
    
    public string LastName { get; init; } = default!;
    
    public string Email { get; init; } = default!;
    
    public string PasswordHash { get; init; } = default!;
    
    public Money Balance { get; init; } = default!;

    public Currency CurrencyPreference { get; init; } = default!;
    
    public ICollection<IncomeTransaction> IncomeTransactions { get; init; } = new List<IncomeTransaction>();
    
    public ICollection<ExpenseTransaction> ExpenseTransactions { get; init; } = new List<ExpenseTransaction>();
}
