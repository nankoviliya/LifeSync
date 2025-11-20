using LifeSync.API.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LifeSync.Tests.Integration.Infrastructure;

public abstract class IntegrationTestsBase : IClassFixture<IntegrationTestsWebAppFactory>, IAsyncLifetime
{
    private readonly IServiceScope _scope;

    protected HttpClient HttpClient { get; }
    protected ApplicationDbContext DbContext { get; }

    protected IntegrationTestsBase(IntegrationTestsWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();

        HttpClient = factory.CreateClient();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (DbContext.Database.GetPendingMigrations().Any())
        {
            DbContext.Database.Migrate();
        }
    }


    public virtual Task InitializeAsync() => Task.CompletedTask;

    public virtual async Task DisposeAsync()
    {
        _scope.Dispose();
        await DbContext.DisposeAsync();
    }
}