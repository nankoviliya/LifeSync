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

    public DbSet<Language> Languages { get; set; }

    public DbSet<Currency> Currencies { get; set; }

    public DbSet<ExpenseTransaction> ExpenseTransactions { get; set; }

    public DbSet<IncomeTransaction> IncomeTransactions { get; set; }

    public DbSet<User> Users { get; set; }
}