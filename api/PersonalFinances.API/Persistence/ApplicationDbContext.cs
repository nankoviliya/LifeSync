using Microsoft.EntityFrameworkCore;
using PersonalFinances.API.Models;
using PersonalFinances.API.Secrets;

namespace PersonalFinances.API.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly ISecretsManager secretsManager;

    public ApplicationDbContext(ISecretsManager secretsManager)
    {
        this.secretsManager = secretsManager;
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

    public DbSet<ExpenseTransaction> ExpenseTransactions { get; set; }
    
    public DbSet<IncomeTransaction> IncomeTransactions { get; set; }
    
    public DbSet<User> Users { get; set; }
}