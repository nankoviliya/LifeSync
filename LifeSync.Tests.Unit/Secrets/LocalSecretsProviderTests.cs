using FluentAssertions;
using LifeSync.API.Secrets;
using LifeSync.API.Secrets.Common;
using LifeSync.API.Secrets.Models;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace LifeSync.UnitTests.Secrets
{
    public class LocalSecretsProviderTests
    {
        private LocalSecretsProvider localSecretsProvider;

        private IConfiguration configuration;

        private readonly AppSecrets expectedSecrets = new AppSecrets
        {
            Database = new DbConnectionSecrets
            {
                DbInstanceIdentifier = "TestDb"
            },
            JWT = new JwtSecrets
            {
                SecretKey = "TestSigningKey",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpiryMinutes = 10
            }
        };

        private IConfiguration CreateValidMockConfiguration()
        {
            var configurationJson = @"
            {
                ""AppSecrets"": {
                    ""Database"": {
                        ""DbInstanceIdentifier"": ""TestDb""
                    },
                    ""JWT"": {
                        ""SecretKey"": ""TestSigningKey"",
                        ""Issuer"": ""TestIssuer"",
                        ""Audience"": ""TestAudience"",
                        ""ExpiryMinutes"": 10
                    }
                }
            }";

            var builder = new ConfigurationBuilder()
                .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(configurationJson)));

            return builder.Build();
        }

        private IConfiguration CreateInvalidMockConfiguration()
        {
            var configurationJson = @"{}";

            var builder = new ConfigurationBuilder()
                .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(configurationJson)));

            return builder.Build();
        }

        [Fact]
        public async Task GetAppSecretsAsync_ShouldReturnAppSecrets_WhenResponseIsNotNull()
        {
            configuration = CreateValidMockConfiguration();
            localSecretsProvider = new LocalSecretsProvider(this.configuration);

            var result = await localSecretsProvider.GetAppSecretsAsync();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedSecrets);
        }

        [Fact]
        public async Task GetAppSecretsAsync_ShouldThrowException_WhenAppSecretsAreNull()
        {
            configuration = CreateInvalidMockConfiguration();
            localSecretsProvider = new LocalSecretsProvider(this.configuration);

            Func<Task> act = async () => await localSecretsProvider.GetAppSecretsAsync();

            await act.Should().ThrowAsync<Exception>()
                .WithMessage(SecretsConstants.ApplicationSecretsNotFoundMessage);
        }
    }
}
