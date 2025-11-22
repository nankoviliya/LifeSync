using FluentAssertions;
using LifeSync.API.Features.FrontendSettings.Models;
using LifeSync.Tests.Integration.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace LifeSync.Tests.Integration.Features.FrontendSettings;

public class GetFrontendSettingsEndpointTests : IntegrationTestsBase
{
    public GetFrontendSettingsEndpointTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Get_ReturnsFrontendSettings_WhenSettingsExist()
    {
        HttpResponseMessage response = await HttpClient.GetAsync($"/api/frontendSettings");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        FrontendSettingsResponse? settings =
            await response.Content.ReadFromJsonAsync<FrontendSettingsResponse>();

        settings.Should().NotBeNull();
        settings.CurrencyOptions.Should().NotBeEmpty();
        settings.LanguageOptions.Should().NotBeEmpty();
    }
}