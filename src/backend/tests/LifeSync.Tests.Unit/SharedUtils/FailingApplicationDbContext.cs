using LifeSync.API.Persistence;
using LifeSync.API.Secrets.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.UnitTests.SharedUtils;

public class FailingApplicationDbContext : ApplicationDbContext
{
    private bool _saveChangesShouldFail;

    public FailingApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ISecretsManager secretsManager)
        : base(options, secretsManager)
    {
    }

    public void SetSaveChangesShouldFail(bool value) => _saveChangesShouldFail = value;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_saveChangesShouldFail)
        {
            throw new DbUpdateException("Simulated database error");
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}