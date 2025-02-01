using LifeSync.API.Models.Languages;
using LifeSync.API.Shared;
using Microsoft.AspNetCore.Identity;

namespace LifeSync.API.Models.ApplicationUser;

public class User : IdentityUser
{
    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string PasswordSalt { get; init; } = default!;

    public Money Balance { get; set; } = default!;

    public Currency CurrencyPreference { get; set; } = default!;

    public Guid LanguageId { get; set; }

    public Language Language { get; init; } = default!;

    public ICollection<IncomeTransaction> IncomeTransactions { get; init; } = [];

    public ICollection<ExpenseTransaction> ExpenseTransactions { get; init; } = [];

    // public ICollection<UserWallet> UserWallets { get; init; } = new List<UserWallet>();
}
