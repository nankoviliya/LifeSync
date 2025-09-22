using FluentAssertions;
using LifeSync.API.Secrets;
using LifeSync.API.Secrets.Common;
using LifeSync.API.Secrets.Contracts;
using LifeSync.API.Secrets.Models;
using NSubstitute;

namespace LifeSync.UnitTests.Secrets;

public class SecretsManagerTests
{
    private readonly SecretsManager secretsManager;
    private readonly ISecretsProvider secretsProvider;

    private readonly AppSecrets expectedBaseSecrets = new AppSecrets
    {
        IsDocker = false,
        DbName = "TestDb",
        DbHost = "TestDbHost",
        DbPasswd = "TestDbPasswd",
        DbPort = 1433,
        DbUser = "TestDbUser",
        JwtSecretKey = "TestSigningKey",
        JwtIssuer = "TestIssuer",
        JwtAudience = "TestAudience",
        JwtExpiryMinutes = 10
    };
    
    private readonly AppSecrets expectedDockerSecrets = new AppSecrets
    {
        IsDocker = true,
        DbName = "TestDb",
        DbHost = "TestDbHost",
        DbPasswd = "TestDbPasswd",
        DbPort = 1433,
        DbUser = "TestDbUser",
        JwtSecretKey = "TestSigningKey",
        JwtIssuer = "TestIssuer",
        JwtAudience = "TestAudience",
        JwtExpiryMinutes = 10
    };

    public SecretsManagerTests()
    {
        secretsProvider = Substitute.For<ISecretsProvider>();

        secretsManager = new SecretsManager(secretsProvider);
    }

    [Fact]
    public async Task GetConnectionStringAsync_ShouldReturnConnectionString_WhenResponseIsNotNull()
    {
        secretsProvider.GetAppSecretsAsync()
            .Returns(Task.FromResult(expectedBaseSecrets));

        var connectionString = await secretsManager.GetConnectionStringAsync();

        connectionString.Should().Be("Server=localhost;Database=TestDb;Trusted_Connection=True;TrustServerCertificate=True;");
    }
    
    [Fact]
    public async Task GetConnectionStringAsync_ShouldReturnDockerConnectionString_WhenDockerEnvSelected()
    {
        secretsProvider.GetAppSecretsAsync()
            .Returns(Task.FromResult(expectedDockerSecrets));

        var connectionString = await secretsManager.GetConnectionStringAsync();

        connectionString.Should().Be("Data Source=TestDbHost,1433;Initial Catalog=TestDb;User Id=TestDbUser;Password=TestDbPasswd;TrustServerCertificate=True;Integrated Security=False;");
    }

    [Fact]
    public async Task GetConnectionStringAsync_ShouldThrowException_WhenAppSecretsIsNull()
    {
        secretsProvider.GetAppSecretsAsync()
            .Returns(Task.FromResult((AppSecrets)null));

        Func<Task> act = async () => await secretsManager.GetConnectionStringAsync();

        await act.Should().ThrowAsync<Exception>()
            .WithMessage(SecretsConstants.DatabaseSecretsNotFoundMessage);
    }

    [Fact]
    public async Task GetConnectionStringAsync_ShouldThrowException_WhenDbSecretsIsNull()
    {
        secretsProvider.GetAppSecretsAsync()
            .Returns(Task.FromResult<AppSecrets>(null));

        Func<Task> act = async () => await secretsManager.GetConnectionStringAsync();

        await act.Should().ThrowAsync<Exception>()
            .WithMessage(SecretsConstants.DatabaseSecretsNotFoundMessage);
    }

    [Fact]
    public async Task GetJwtSecretsAsync_ReturnsJwtSecrets_WhenSecretStringIsValid()
    {
        secretsProvider.GetAppSecretsAsync()
            .Returns(Task.FromResult(expectedBaseSecrets));

        var result = await secretsManager.GetJwtSecretsAsync();

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedBaseSecrets.JWT);
    }

    [Fact]
    public async Task GetJwtSecretsAsync_ShouldThrowException_WhenAppSecretsIsNull()
    {
        secretsProvider.GetAppSecretsAsync()
            .Returns(Task.FromResult((AppSecrets)null));

        Func<Task> act = async () => await secretsManager.GetJwtSecretsAsync();

        await act.Should().ThrowAsync<Exception>()
            .WithMessage(SecretsConstants.JWTSecretsNotFoundMessage);
    }

    [Fact]
    public async Task GetJwtSecretsAsync_ShouldThrowException_WhenDbSecretsIsNull()
    {
        secretsProvider.GetAppSecretsAsync()
            .Returns(Task.FromResult<AppSecrets>(null));

        Func<Task> act = async () => await secretsManager.GetJwtSecretsAsync();

        await act.Should().ThrowAsync<Exception>()
            .WithMessage(SecretsConstants.JWTSecretsNotFoundMessage);
    }

}