namespace LifeSync.API.Secrets.Contracts
{
    public interface ISecretsProviderFactory
    {
        ISecretsProvider CreateSecretsProvider();
    }
}
