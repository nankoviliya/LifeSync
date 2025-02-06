using LifeSync.API.Secrets.Models;

namespace LifeSync.API.Secrets.Contracts
{
    public interface ISecretsProvider
    {
        Task<AppSecrets> GetAppSecretsAsync();
    }
}
