using Microsoft.EntityFrameworkCore;
using PersonalFinances.API.Infrastructure.DomainEvents;
using PersonalFinances.API.Models;
using PersonalFinances.API.Models.Abstractions;
using PersonalFinances.API.Models.ApplicationUser;
using PersonalFinances.API.Secrets;

namespace PersonalFinances.API.Persistence;

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
        var connectionString = secretsManager.GetConnectionStringAsync().Result;
        optionsBuilder.UseSqlServer(connectionString);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync();
            
        return result;        
    }

    private async Task PublishDomainEventsAsync()
    {
        var entities = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)

            .ToList();

        var domainEvents = entities.SelectMany(entity =>
        {
            var domainEvents = entity.GetDomainEvents();

            return domainEvents;
        }).ToList();
        
        foreach (var entity in entities)
        {
            entity.ClearDomainEvents();
        }
        
        await eventDispatcher.DispatchAsync(domainEvents);
    }

    public DbSet<ExpenseTransaction> ExpenseTransactions { get; set; }
    
    public DbSet<IncomeTransaction> IncomeTransactions { get; set; }
    
    public DbSet<User> Users { get; set; }
}