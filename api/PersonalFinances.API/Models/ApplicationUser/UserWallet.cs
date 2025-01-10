using PersonalFinances.API.Models.Abstractions;
using PersonalFinances.API.Shared;

namespace PersonalFinances.API.Models.ApplicationUser;

public class UserWallet : Entity
{
    public Guid UserId { get; set; }

    public User User { get; set; } = default!;

    public string Name { get; set; } = default!;

    public Money Balance { get; set; } = default!;
}