using LifeSync.API.Features.Authentication.Models;

namespace LifeSync.API.Secrets;

public interface ISecretsManager
{
    Task<string> GetConnectionStringAsync();
    
    Task<JwtSecrets> GetJwtSecretAsync();
}