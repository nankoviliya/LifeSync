using System.Text.Json;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using PersonalFinances.API.Features.Authentication.Models;

namespace PersonalFinances.API.Secrets;

/// <summary>
/// Secrets are currently in local storage.
/// </summary>
/// <param name="secretsManager"></param>
/// <param name="configuration"></param>
/// <param name="_hostEnvironment"></param>
public class SecretsManager(IConfiguration configuration, IHostEnvironment _hostEnvironment)
    : ISecretsManager
{
    public async Task<string> GetConnectionStringAsync()
    {
        // string secretName = configuration["SecretsManager"];
        //
        // var request = new GetSecretValueRequest
        // {
        //     SecretId = secretName,
        //     VersionStage = "AWSCURRENT",
        // };
        //
        // var response = await secretsManager.GetSecretValueAsync(request);
        //

        var dbSecret = configuration.GetSection("Database").Get<DbConnectionSecrets>();
        
        if (dbSecret is not null)
        {
            var devConnectionString = $"Server=localhost;" +
                                   $"Database={dbSecret.DbInstanceIdentifier};" +
                                   $"Trusted_Connection=True;" +
                                   $"TrustServerCertificate=True;";
    
            return devConnectionString;
        }

        throw new Exception("Secret not found or is in binary format.");
    }

    public async Task<JwtSecrets> GetJwtSecretAsync()
    {
        // string secretName = configuration["SecretsManager"];
        //
        // var request = new GetSecretValueRequest
        // {
        //     SecretId = secretName,
        //     VersionStage = "AWSCURRENT",
        // };
        //
        // var response = await secretsManager.GetSecretValueAsync(request);
        
        var jwtSecret = configuration.GetSection("JWT").Get<JwtSecrets>();
        
        if (jwtSecret is not null)
        {
            return jwtSecret;
        }

        throw new Exception("Secret not found or is in binary format.");
    }
}