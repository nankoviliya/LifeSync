using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using FluentAssertions;
using LifeSync.API.Secrets;
using LifeSync.API.Secrets.Common;
using LifeSync.API.Secrets.Exceptions;
using LifeSync.API.Secrets.Models;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Text;

namespace LifeSync.UnitTests.Secrets;

public class CloudSecretsProviderTests
{
    private CloudSecretsProvider cloudSecretsProvider;

    private IConfiguration configuration;
    private IAmazonSecretsManager amazonSecretsManager;

    private string MockCloudSecretsConfigurationJson = @"
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
        var configurationJson = @"{
                ""SecretName"": ""LifeSync"",
            }";

        var builder = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(configurationJson)));

        return builder.Build();
    }

    private IConfiguration CreateInvalidMockConfiguration()
    {
        var configurationJson = @"{
                ""SecretName"": """",
            }";

        var builder = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(configurationJson)));

        return builder.Build();
    }

    [Fact]
    public async Task GetAppSecretsAsync_ShouldReturnAppSecrets_WhenResponseIsNotNull()
    {
        configuration = CreateValidMockConfiguration();
        amazonSecretsManager = Substitute.For<IAmazonSecretsManager>();
        cloudSecretsProvider = new CloudSecretsProvider(this.configuration, this.amazonSecretsManager);

        var secretResponse = new GetSecretValueResponse
        {
            SecretString = MockCloudSecretsConfigurationJson,
        };

        amazonSecretsManager.GetSecretValueAsync(Arg.Any<GetSecretValueRequest>()).Returns(Task.FromResult(secretResponse));

        var result = await cloudSecretsProvider.GetAppSecretsAsync();

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedSecrets);
    }

    [Fact]
    public async Task GetAppSecretsAsync_ShouldThrowArgumentException_WhenSecretNameIsNullOrEmpty()
    {
        configuration = CreateInvalidMockConfiguration();
        amazonSecretsManager = Substitute.For<IAmazonSecretsManager>();
        cloudSecretsProvider = new CloudSecretsProvider(this.configuration, this.amazonSecretsManager);

        Func<Task> act = async () => await cloudSecretsProvider.GetAppSecretsAsync();

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage(SecretsConstants.SecretNameIsNotConfiguredMessage);
    }

    [Fact]
    public async Task GetAppSecretsAsync_ShouldThrowSecretsRetrievalException_WhenSecretStringIsNullOrEmpty()
    {
        configuration = CreateValidMockConfiguration();
        amazonSecretsManager = Substitute.For<IAmazonSecretsManager>();
        cloudSecretsProvider = new CloudSecretsProvider(this.configuration, this.amazonSecretsManager);

        var secretResponse = new GetSecretValueResponse
        {
            SecretString = "",
        };

        amazonSecretsManager.GetSecretValueAsync(Arg.Any<GetSecretValueRequest>()).Returns(Task.FromResult(secretResponse));

        Func<Task> act = async () => await cloudSecretsProvider.GetAppSecretsAsync();

        await act.Should().ThrowAsync<SecretsRetrievalException>()
            .WithMessage(SecretsConstants.ApplicationSecretsRetrievalErrorMessage);
    }

    [Fact]
    public async Task GetAppSecretsAsync_ShouldThrowSecretsRetrievalException_WhenSecretStringDeserializationFailed()
    {
        configuration = CreateValidMockConfiguration();
        amazonSecretsManager = Substitute.For<IAmazonSecretsManager>();
        cloudSecretsProvider = new CloudSecretsProvider(this.configuration, this.amazonSecretsManager);

        var secretResponse = new GetSecretValueResponse
        {
            SecretString = "INVALID_SECRET",
        };

        amazonSecretsManager.GetSecretValueAsync(Arg.Any<GetSecretValueRequest>()).Returns(Task.FromResult(secretResponse));

        Func<Task> act = async () => await cloudSecretsProvider.GetAppSecretsAsync();

        await act.Should().ThrowAsync<SecretsRetrievalException>()
            .WithMessage(SecretsConstants.ApplicationSecretsRetrievalErrorMessage);
    }
}
