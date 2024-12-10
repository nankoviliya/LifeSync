using PersonalFinances.API.Features.Authentication.Models;

namespace PersonalFinances.API.Secrets;

public interface ISecretsManager
{
    Task<string> GetConnectionStringAsync();
    
    Task<JwtSecrets> GetJwtSecretAsync();
}