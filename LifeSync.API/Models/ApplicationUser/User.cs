using Microsoft.AspNetCore.Identity;
using LifeSync.API.Shared;

namespace LifeSync.API.Models.ApplicationUser;

public class User : IdentityUser
{
    public string FirstName { get; init; } = default!;
    
    public string LastName { get; init; } = default!;
    
    
    public string PasswordSalt { get; init; } = default!;
    
    public Money Balance { get; set; } = default!;

    public Currency CurrencyPreference { get; init; } = default!;
    
    public ICollection<IncomeTransaction> IncomeTransactions { get; init; } = new List<IncomeTransaction>();
    
    public ICollection<ExpenseTransaction> ExpenseTransactions { get; init; } = new List<ExpenseTransaction>();
    
    // public ICollection<UserWallet> UserWallets { get; init; } = new List<UserWallet>();
}
