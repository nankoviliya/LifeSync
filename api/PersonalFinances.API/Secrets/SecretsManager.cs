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
        string secretName = configuration["SecretsManager"];
        
        var request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT",
        };

        var response = await secretsManager.GetSecretValueAsync(request);
        
        if (response.SecretString is not null)
        {
            var appSecrets = JsonSerializer.Deserialize<AppSecrets>(response.SecretString);

            var databaseSecret = appSecrets.Database;
            
            var devConnectionString = $"Server=localhost;" +
                                   $"Database={databaseSecret.DbInstanceIdentifier};" +
                                   $"Trusted_Connection=True;" +
                                   $"TrustServerCertificate=True;";
    
            return devConnectionString;
        }

        throw new Exception("Secret not found or is in binary format.");
    }

    public async Task<JwtSecrets> GetJwtSecretAsync()
    {
        string secretName = configuration["SecretsManager"];
        
        var request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT",
        };

        var response = await secretsManager.GetSecretValueAsync(request);
        
        if (response.SecretString != null)
        {
            var appSecrets = JsonSerializer.Deserialize<AppSecrets>(response.SecretString);

            return appSecrets.JWT;
        }

        throw new Exception("Secret not found or is in binary format.");
    }
}