using System.Text.Json;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace PersonalFinances.API.Secrets;

public class SecretsManager(IAmazonSecretsManager secretsManager, IConfiguration configuration)
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
            
            var connectionString = $"Server={databaseSecret.Host},{databaseSecret.Port};" +
                                   $"Database={databaseSecret.DbInstanceIdentifier};" +
                                   $"User Id={databaseSecret.Username};" +
                                   $"Password={databaseSecret.Password};" +
                                   $"Encrypt=True;" + 
                                   $"TrustServerCertificate=True;";

            return connectionString;
        }

        throw new Exception("Secret not found or is in binary format.");
    }
}