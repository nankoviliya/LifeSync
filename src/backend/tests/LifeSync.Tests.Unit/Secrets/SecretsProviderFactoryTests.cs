using Amazon.SecretsManager;
using FluentAssertions;
using LifeSync.API.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace LifeSync.Tests.Unit.Secrets;

public class SecretsProviderFactoryTests
{
    private IHostEnvironment environment;
    private IServiceProvider serviceProvider;
    private SecretsProviderFactory factory;

    public SecretsProviderFactoryTests()
    {
        environment = Substitute.For<IHostEnvironment>();

        var serviceCollection = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string> {
            { "AWS:Region", "us-east-1" },
            { "AWS:Profile", "default" },
        })
        .Build();

        serviceCollection.AddSingleton<IConfiguration>(configuration);

        // Register AWS Secrets Service
        var amazonSecretsManager = Substitute.For<IAmazonSecretsManager>();

        serviceCollection.AddDefaultAWSOptions(configuration.GetAWSOptions());
        serviceCollection.AddAWSService<IAmazonSecretsManager>();
        serviceCollection.AddTransient<CloudSecretsProvider>();

        // Register Local Secrets Service
        serviceCollection.AddTransient<LocalSecretsProvider>();

        serviceProvider = serviceCollection.BuildServiceProvider();

        factory = new SecretsProviderFactory(environment, serviceProvider);
    }

    [Theory]
    [InlineData("Development", typeof(LocalSecretsProvider))]
    [InlineData("Production", typeof(CloudSecretsProvider))]
    public void CreateSecretsProvider_ShouldReturnExpectedProvider(string environmentName, Type expectedType)
    {
        environment.EnvironmentName.Returns(environmentName);

        var result = factory.CreateSecretsProvider();

        result.Should().NotBeNull();
        result.Should().BeOfType(expectedType);
    }
}
