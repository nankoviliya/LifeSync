using FluentAssertions;
using LifeSync.API;
using LifeSync.API.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace LifeSync.Tests.Integration.Features.Translations;

public class GetTranslationsEndpointTests : IClassFixture<WebApplicationFactory<IApiMarker>>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<IApiMarker> _factory;

    private List<string> _supportedLanguageCodes;

    public GetTranslationsEndpointTests(WebApplicationFactory<IApiMarker> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        _supportedLanguageCodes = await context.Languages
            .Select(l => l.Code)
            .ToListAsync();
    }

    [Fact]
    public async Task Get_ReturnsDictionaryOfStrings_ForSupportedLanguage()
    {
        foreach (string code in _supportedLanguageCodes)
        {
            HttpResponseMessage response = await _client.GetAsync($"/api/translations/{code}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            IReadOnlyDictionary<string, string>? translations =
                await response.Content.ReadFromJsonAsync<IReadOnlyDictionary<string, string>>();

            translations.Should().NotBeNull();
            translations.Should().NotBeEmpty($"Translations for '{code}' should not be empty");
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;
}