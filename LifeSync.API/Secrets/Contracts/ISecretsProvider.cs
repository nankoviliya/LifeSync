namespace LifeSync.API.Secrets.Contracts
{
    public interface ISecretsProvider
    {
        Task<AppSecrets> GetAppSecretsAsync();
    }
}
