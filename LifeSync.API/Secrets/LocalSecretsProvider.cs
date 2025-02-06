using LifeSync.API.Secrets.Common;
using LifeSync.API.Secrets.Contracts;
using LifeSync.API.Secrets.Models;

namespace LifeSync.API.Secrets
{
    public class LocalSecretsProvider : ISecretsProvider
    {
        private readonly IConfiguration configuration;

        public LocalSecretsProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Task<AppSecrets> GetAppSecretsAsync()
        {
            var appSecrets = configuration.GetSection("AppSecrets").Get<AppSecrets>();

            if (appSecrets is null)
            {
                throw new Exception(SecretsConstants.ApplicationSecretsNotFoundMessage);
            }

            return Task.FromResult(appSecrets);
        }
    }
}
