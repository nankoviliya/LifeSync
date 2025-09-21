using FluentAssertions;
using LifeSync.API.Secrets;
using LifeSync.API.Secrets.Common;
using LifeSync.API.Secrets.Contracts;
using LifeSync.API.Secrets.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Text;

namespace LifeSync.UnitTests.Secrets;

public class LocalSecretsProviderTests
{
    private LocalSecretsProvider localSecretsProvider;

    private IConfiguration configuration;

    private readonly AppSecrets expectedSecrets = new AppSecrets
    {
        DbName = "TestDb",
        DbUser = "admin",
        DbPasswd = "YourStrongPassword123!",
        DbHost = "localhost",
        DbPort = 1433,
        JwtSecretKey = "TestSigningKey",
        JwtIssuer = "TestIssuer",
        JwtAudience = "TestAudience",
        JwtExpiryMinutes = 10
    };

    private IConfiguration CreateValidMockConfiguration()
    {
        var configurationJson = @"
            {
                ""JWT_SECRET_KEY"": ""TestSigningKey"",
                ""JWT_ISSUER"": ""TestIssuer"",
                ""JWT_AUDIENCE"": ""TestAudience"",
                ""JWT_EXPIRY_MINUTES"": 10,
                ""DB_USER"": ""admin"",
                ""DB_PASSWD"": ""YourStrongPassword123!"",
                ""DB_HOST"": ""localhost"",
                ""DB_PORT"": 1433,
                ""DB_NAME"": ""TestDb""
            }";

        var builder = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(configurationJson)));

        return builder.Build();
    }

    private IConfiguration CreateInvalidMockConfiguration()
    {
        var configurationJson = @"{}";

        var builder = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(configurationJson)));

        return builder.Build();
    }

    [Fact]
    public async Task GetAppSecretsAsync_ShouldReturnAppSecrets_WhenResponseIsNotNull()
    {
        configuration = CreateValidMockConfiguration();
        localSecretsProvider = new LocalSecretsProvider(configuration);

        var result = await localSecretsProvider.GetAppSecretsAsync();

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedSecrets);
    }

    [Fact]
    public async Task GetAppSecretsAsync_ShouldThrowException_WhenAppSecretsAreNull()
    {
        configuration = CreateInvalidMockConfiguration();
        localSecretsProvider = new LocalSecretsProvider(configuration);

        Func<Task> act = async () => await localSecretsProvider.GetAppSecretsAsync();

        await act.Should().ThrowAsync<Exception>()
            .WithMessage(SecretsConstants.ApplicationSecretsRetrievalErrorMessage);
    }
}
