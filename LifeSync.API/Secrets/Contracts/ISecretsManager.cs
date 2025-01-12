namespace LifeSync.API.Secrets.Contracts;

/// <summary>
/// Defines base interface for application
/// secrets management
/// </summary>
public interface ISecretsManager
{
    Task<string> GetConnectionStringAsync();

    Task<JwtSecrets> GetJwtSecretsAsync();
}