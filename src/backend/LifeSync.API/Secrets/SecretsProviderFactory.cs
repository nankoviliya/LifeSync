using LifeSync.API.Secrets.Contracts;

namespace LifeSync.API.Secrets;

public class SecretsProviderFactory : ISecretsProviderFactory
{
    private readonly IHostEnvironment environment;
    private readonly IServiceProvider serviceProvider;

    public SecretsProviderFactory(
        IHostEnvironment environment,
        IServiceProvider serviceProvider)
    {
        this.environment = environment;
        this.serviceProvider = serviceProvider;
    }

    public ISecretsProvider CreateSecretsProvider()
    {
        if (environment.IsDevelopment())
        {
            return serviceProvider.GetRequiredService<LocalSecretsProvider>();
        }
        else
        {
            return serviceProvider.GetRequiredService<CloudSecretsProvider>();
        }
    }
}
