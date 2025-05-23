using LifeSync.API.Infrastructure.DomainEvents;
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
    private readonly IDomainEventDispatcher eventDispatcher;

    public ApplicationDbContext(
        DbContextOptions options,
        IDomainEventDispatcher eventDispatcher,
        ISecretsManager secretsManager) : base(options)
    {
        this.secretsManager = secretsManager;
        this.eventDispatcher = eventDispatcher;
    }

    public ApplicationDbContext()
    {

    }

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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(cancellationToken);

        if (eventDispatcher is not null)
        {
            await PublishDomainEventsAsync();
        }

        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        var entitiesWithEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .Where(entity => entity.GetDomainEvents().Any())
            .ToList();

        var domainEvents = entitiesWithEvents
                .SelectMany(entity => entity.GetDomainEvents())
                .ToList();

        entitiesWithEvents.ForEach(entity => entity.ClearDomainEvents());

        await eventDispatcher.DispatchAsync(domainEvents);
    }

    public DbSet<Language> Languages { get; set; }

    public DbSet<Currency> Currencies { get; set; }

    public DbSet<ExpenseTransaction> ExpenseTransactions { get; set; }

    public DbSet<IncomeTransaction> IncomeTransactions { get; set; }

    public DbSet<User> Users { get; set; }
}