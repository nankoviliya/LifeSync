using System.Text.Json;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using PersonalFinances.API.Features.Authentication.Models;

namespace PersonalFinances.API.Secrets;

public class SecretsManager(IAmazonSecretsManager secretsManager, IConfiguration configuration, IHostEnvironment _hostEnvironment)
    : ISecretsManager
{
    public async Task<string> GetConnectionStringAsync()
    {
        string secretName = configuration["SecretsManager:ConnectionStringSecretName"];
        
        var request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT",
        };

        var response = await secretsManager.GetSecretValueAsync(request);
        if (response.SecretString != null)
        {
            var databaseSecret = JsonSerializer.Deserialize<DbConnectionSecret>(response.SecretString);

            if (_hostEnvironment.IsDevelopment())
            {
                var devConnectionString = $"Server=localhost;" +
                                       $"Database={databaseSecret.DbInstanceIdentifier};" +
                                       $"Trusted_Connection=True;" +
                                       $"TrustServerCertificate=True;";
        
                return devConnectionString;
            }
            
            var connectionString = $"Server={databaseSecret.Host},{databaseSecret.Port};" +
                                   $"Database={databaseSecret.DbInstanceIdentifier};" +
                                   $"User Id={databaseSecret.Username};" +
                                   $"Password={databaseSecret.Password};" +
                                   $"Encrypt=True;" +
                                   $"TrustServerCertificate=False;";
        
            return connectionString;
        }

        throw new Exception("Secret not found or is in binary format.");
    }

    public async Task<JwtSettings> GetJwtSettingsAsync()
    {
        string secretName = configuration["SecretsManager:JwtSettingsSecretName"];
        
        var request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT",
        };

        var response = await secretsManager.GetSecretValueAsync(request);
        if (response.SecretString != null)
        {
            var jwtSettings = JsonSerializer.Deserialize<JwtSettings>(response.SecretString);

            return jwtSettings;
        }

        throw new Exception("Secret not found or is in binary format.");
    }
}