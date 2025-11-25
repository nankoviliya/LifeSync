using LifeSync.API;
using LifeSync.API.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace LifeSync.Tests.Integration.Infrastructure;

public class IntegrationTestsWebAppFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _databaseContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server")
        .WithEnvironment("DB_PORT", "1433")
        .WithEnvironment("DB_HOST", "localhost")
        .WithEnvironment("DB_NAME", "LifeSync_Test")
        .WithEnvironment("DB_USER", "SA")
        .WithEnvironment("DB_PASSWD", "YourStrongPassword123!")
        .Build();

    public async Task InitializeAsync() => await _databaseContainer.StartAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.ConfigureTestServices(services =>
        {
            ServiceDescriptor? descriptor =
                services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options
                    .UseSqlServer(_databaseContainer.GetConnectionString());
            });
        });

    public new async Task DisposeAsync()
    {
        await _databaseContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}