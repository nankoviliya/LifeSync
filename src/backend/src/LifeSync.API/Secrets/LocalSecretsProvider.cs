using LifeSync.API.Secrets.Common;
using LifeSync.API.Secrets.Contracts;
using LifeSync.API.Secrets.Exceptions;
using LifeSync.API.Secrets.Models;

namespace LifeSync.API.Secrets;

public class LocalSecretsProvider : ISecretsProvider
{
    private readonly IConfiguration _configuration;

    public LocalSecretsProvider(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public Task<AppSecrets> GetAppSecretsAsync()
    {
        try
        {
            var appSecrets = new AppSecrets
            {
                // TODO: Find a way to fetch all once
                IsDocker = _configuration.GetValue<bool?>("DOCKER") ?? throw new InvalidOperationException("DOCKER configuration is required"),
                DbUser = _configuration["DB_USER"] ?? throw new InvalidOperationException("DB_USER configuration is required"),
                DbPasswd = _configuration["DB_PASSWD"] ?? throw new InvalidOperationException("DB_PASSWD configuration is required"),
                DbHost = _configuration["DB_HOST"] ?? throw new InvalidOperationException("DB_HOST configuration is required"),
                DbPort = _configuration.GetValue<int?>("DB_PORT") ?? throw new InvalidOperationException("DB_PORT configuration is required"),
                DbName = _configuration["DB_NAME"] ?? throw new InvalidOperationException("DB_NAME configuration is required"),
                JwtSecretKey = _configuration["JWT_SECRET_KEY"] ?? throw new InvalidOperationException("JWT_SECRET_KEY configuration is required"),
                JwtIssuer = _configuration["JWT_ISSUER"] ?? throw new InvalidOperationException("JWT_ISSUER configuration is required"),
                JwtAudience = _configuration["JWT_AUDIENCE"] ?? throw new InvalidOperationException("JWT_AUDIENCE configuration is required"),
                JwtExpiryMinutes = _configuration.GetValue<int?>("JWT_EXPIRY_MINUTES") ?? throw new InvalidOperationException("JWT_EXPIRY_MINUTES configuration is required")
            };
            
            return Task.FromResult(appSecrets);
        }
        catch (Exception ex)
        {
            throw new SecretsRetrievalException(SecretsConstants.ApplicationSecretsRetrievalErrorMessage, ex);
        }
    }
}
