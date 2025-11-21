using LifeSync.API.Models.ApplicationUser;
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

    protected virtual Task OnInitializeAsync() => Task.CompletedTask;
    protected virtual Task OnDisposeAsync() => Task.CompletedTask;

    protected User GetDefaultUser() =>
        DbContext.Users.FirstOrDefault(x => x.Email == DefaultUserAccount.RegisterUserRequest.Email) ??
        throw new InvalidOperationException("Default user should be inserted");

    public async Task InitializeAsync()
    {
        await HttpClient.InsertDefaultUserAsync(DefaultUserAccount.RegisterUserRequest);
        await OnInitializeAsync();
    }

    public async Task DisposeAsync()
    {
        try
        {
            await OnDisposeAsync();
        }
        finally
        {
            _scope.Dispose();
        }
    }
}