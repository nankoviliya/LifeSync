using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Currencies;
using LifeSync.API.Models.Expenses;
using LifeSync.API.Models.Incomes;
using LifeSync.API.Models.Languages;
using LifeSync.API.Secrets.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly ISecretsManager secretsManager;

    public ApplicationDbContext(
        DbContextOptions options,
        ISecretsManager secretsManager) : base(options)
    {
        this.secretsManager = secretsManager;
    }

#pragma warning disable CS8618
    public ApplicationDbContext() { }
#pragma warning restore CS8618

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && secretsManager is not null)
        {
            var connectionString = secretsManager.GetConnectionStringAsync().GetAwaiter().GetResult();
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<Entity>();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(Entity.CreatedAt)).CurrentValue = now;
                entry.Property(nameof(Entity.UpdatedAt)).CurrentValue = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(Entity.UpdatedAt)).CurrentValue = now;
            }
        }
    }

    public DbSet<Language> Languages { get; set; }

    public DbSet<Currency> Currencies { get; set; }

    public DbSet<ExpenseTransaction> ExpenseTransactions { get; set; }

    public DbSet<IncomeTransaction> IncomeTransactions { get; set; }

    public DbSet<User> Users { get; set; }
}