using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using LifeSync.API.Secrets.Common;
using LifeSync.API.Secrets.Contracts;
using LifeSync.API.Secrets.Models;
using LifeSync.API.Shared;
using System.Text.Json;

namespace LifeSync.API.Secrets
{
    public class CloudSecretsProvider : ISecretsProvider
    {
        private readonly IConfiguration configuration;
        private readonly IAmazonSecretsManager secretsManager;

        public CloudSecretsProvider(
            IConfiguration configuration,
            IAmazonSecretsManager secretsManager)
        {
            this.configuration = configuration;
            this.secretsManager = secretsManager;
        }

        public async Task<AppSecrets> GetAppSecretsAsync()
        {
            var secretName = configuration.GetValue<string>(AppConstants.SecretName);

            if (string.IsNullOrEmpty(secretName))
            {
                throw new ArgumentException(SecretsConstants.SecretNameIsNotConfiguredMessage);
            }

            var request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT",
            };

            try
            {
                var response = await secretsManager.GetSecretValueAsync(request);

                if (string.IsNullOrEmpty(response.SecretString))
                {
                    throw new InvalidOperationException(SecretsConstants.ApplicationSecretsNotFoundMessage);
                }

                var appSecrets = JsonSerializer.Deserialize<AppSecrets>(response.SecretString);

                if (appSecrets is null)
                {
                    throw new InvalidOperationException("Deserialized app secrets are null.");
                }

                return appSecrets;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(SecretsConstants.ApplicationSecretsRetrievalErrorMessage, ex);
            }
        }
    }
}
