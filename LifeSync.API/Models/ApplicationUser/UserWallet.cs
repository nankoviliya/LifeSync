using LifeSync.API.Models.Abstractions;
using LifeSync.API.Shared;

namespace LifeSync.API.Models.ApplicationUser;

public class UserWallet : Entity
{
    public Guid UserId { get; set; }

    public User User { get; set; } = default!;

    public string Name { get; set; } = default!;

    public Money Balance { get; set; } = default!;
}