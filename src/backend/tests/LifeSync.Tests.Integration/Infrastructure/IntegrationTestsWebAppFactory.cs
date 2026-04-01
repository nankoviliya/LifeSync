using LifeSync.API;
using LifeSync.API.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace LifeSync.Tests.Integration.Infrastructure;

public class IntegrationTestsWebAppFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _databaseContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17")
        .WithDatabase("LifeSync_Test")
        .WithUsername("postgres")
        .WithPassword("YourStrongPassword123!")
        .Build();

    public async Task InitializeAsync() => await _databaseContainer.StartAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IHostedService));
            
            ServiceDescriptor? descriptor =
                services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options
                    .UseNpgsql(_databaseContainer.GetConnectionString());
            });
        });

    public new async Task DisposeAsync()
    {
        await _databaseContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}