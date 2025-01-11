using System.Text.Json;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using LifeSync.API.Secrets;

namespace LifeSync.UnitTests.Secrets;

public class SecretsManagerTests
{
    private readonly SecretsManager _secretsManager;
    private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();
    private readonly IHostEnvironment _hostEnvironment = Substitute.For<IHostEnvironment>();

    public SecretsManagerTests()
    {
        _secretsManager = new SecretsManager(_configuration, _hostEnvironment);
    }

    [Fact]
    public async Task GetConnectionStringAsync_ShouldReturnConnectionString_WhenResponseIsNotNull()
    {
        var secretName = "TestSecret";
        _configuration["SecretsManager"].Returns(secretName);

        var appSecrets = new AppSecrets
        {
            Database = new DbConnectionSecrets
            {
                DbInstanceIdentifier = "TestDb"
            }
        };

        var secretString = JsonSerializer.Serialize(appSecrets);
        var response = new GetSecretValueResponse { SecretString = secretString };

        // _amazonSecretsManager.GetSecretValueAsync(Arg.Any<GetSecretValueRequest>())
        //     .Returns(Task.FromResult(response));
        
        var connectionString = await _secretsManager.GetConnectionStringAsync();
        
        connectionString.Should().Be("Server=localhost;Database=TestDb;Trusted_Connection=True;TrustServerCertificate=True;");
    }
    
    [Fact]
    public async Task GetConnectionStringAsync_ShouldThrowException_WhenSecretStringIsNull()
    {
        var secretName = "TestSecret";
        
        _configuration["SecretsManager"].Returns(secretName);

        var response = new GetSecretValueResponse { SecretString = null };

        // _amazonSecretsManager.GetSecretValueAsync(Arg.Any<GetSecretValueRequest>())
        //     .Returns(Task.FromResult(response));
        
        Func<Task> act = async () => await _secretsManager.GetConnectionStringAsync();

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Secret not found or is in binary format.");
    }

    [Fact]
    public async Task GetConnectionStringAsync_ShouldThrowException_WhenSecretsManagerKeyIsNotConfigured()
    {
        _configuration["SecretsManager"].Returns((string)null);
        
        Func<Task> act = async () => await _secretsManager.GetConnectionStringAsync();
        
        await act.Should().ThrowAsync<NullReferenceException>();
    }
    
    [Fact]
    public async Task GetJwtSecretAsync_ReturnsJwtSecrets_WhenSecretStringIsValid()
    {
        var secretName = "TestSecret";
        _configuration["SecretsManager"].Returns(secretName);

        var appSecrets = new AppSecrets
        {
            JWT = new JwtSecrets
            {
                SecretKey = "TestSigningKey",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpiryMinutes = 10
            }
        };

        var secretString = JsonSerializer.Serialize(appSecrets);
        var response = new GetSecretValueResponse { SecretString = secretString };

        // _amazonSecretsManager.GetSecretValueAsync(Arg.Any<GetSecretValueRequest>())
        //                    .Returns(Task.FromResult(response));
        
        var result = await _secretsManager.GetJwtSecretAsync();

        // Assert
        result.Should().NotBeNull();
        result.SecretKey.Should().Be("TestSigningKey");
        result.Issuer.Should().Be("TestIssuer");
        result.Audience.Should().Be("TestAudience");
        result.ExpiryMinutes.Should().Be(10);
    }

    [Fact]
    public async Task GetJwtSecretAsync_ThrowsException_WhenSecretStringIsNull()
    {
        var secretName = "TestSecret";
        _configuration["SecretsManager"].Returns(secretName);

        var response = new GetSecretValueResponse { SecretString = null };

        // _amazonSecretsManager.GetSecretValueAsync(Arg.Any<GetSecretValueRequest>())
        //                    .Returns(Task.FromResult(response));
        
        Func<Task> act = async () => await _secretsManager.GetJwtSecretAsync();

        // Assert
        await act.Should().ThrowAsync<Exception>()
                .WithMessage("Secret not found or is in binary format.");
    }

    [Fact]
    public async Task GetJwtSecretAsync_ThrowsArgumentNullException_WhenSecretsManagerKeyIsNotConfigured()
    {
        _configuration["SecretsManager"].Returns((string)null);
        
        Func<Task> act = async () => await _secretsManager.GetJwtSecretAsync();

        // Assert
        await act.Should().ThrowAsync<NullReferenceException>()
                .WithMessage("Object reference not set to an instance of an object.");
    }
}