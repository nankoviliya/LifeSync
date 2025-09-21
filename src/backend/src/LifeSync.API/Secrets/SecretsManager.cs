using LifeSync.API.Secrets.Common;
using LifeSync.API.Secrets.Contracts;
using LifeSync.API.Secrets.Exceptions;
using LifeSync.API.Secrets.Models;

namespace LifeSync.API.Secrets;

/// <summary>
/// Loads secrets from local secrets storage or AWS Secrets Manager
/// based on current application environment
/// To see more about local secrets storage -
/// <see cref="https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-9.0&tabs=windows"/>
/// To see more about AWS Secrets Manager - 
/// <see cref="https://docs.aws.amazon.com/secretsmanager/latest/userguide/retrieving-secrets_cache-net.html"/>
/// </summary>
/// <param name="configuration"></param>
public class SecretsManager(
    ISecretsProvider secretsProvider)
    : ISecretsManager
{
    public async Task<string> GetConnectionStringAsync()
    {
        var appSecrets = await secretsProvider.GetAppSecretsAsync();

        if (appSecrets is null || appSecrets.Database is null)
        {
            throw new SecretsRetrievalException(SecretsConstants.DatabaseSecretsNotFoundMessage);
        }
        
        var dbSecret = appSecrets.Database;

        if (appSecrets.IsDocker)
        {
            return $"Data Source={dbSecret.Host},{dbSecret.Port};" +
                   $"Initial Catalog={dbSecret.DbInstanceIdentifier};" +
                   $"User Id={dbSecret.Username};" +
                   $"Password={dbSecret.Password};" +
                   $"TrustServerCertificate=True;" +
                   $"Integrated Security=False;";
        } 
        
        return $"Server=localhost;" +
               $"Database={dbSecret.DbInstanceIdentifier};" +
               $"Trusted_Connection=True;" +
               $"TrustServerCertificate=True;";
    }

    public async Task<JwtSecrets> GetJwtSecretsAsync()
    {
        var appSecrets = await secretsProvider.GetAppSecretsAsync();

        if (appSecrets is null || appSecrets.JWT is null)
        {
            throw new SecretsRetrievalException(SecretsConstants.JWTSecretsNotFoundMessage);
        }

        return appSecrets.JWT;
    }
}