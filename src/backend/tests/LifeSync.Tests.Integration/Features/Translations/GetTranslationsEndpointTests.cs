using FluentAssertions;
using LifeSync.Tests.Integration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace LifeSync.Tests.Integration.Features.Translations;

public class GetTranslationsEndpointTests : IntegrationTestsBase
{
    private List<string> _supportedLanguageCodes;

    public GetTranslationsEndpointTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    public override async Task InitializeAsync() =>
        _supportedLanguageCodes = await DbContext.Languages
            .Select(l => l.Code)
            .ToListAsync();

    [Fact]
    public async Task Get_ReturnsDictionaryOfStrings_ForSupportedLanguage()
    {
        foreach (string code in _supportedLanguageCodes)
        {
            HttpResponseMessage response = await HttpClient.GetAsync($"/api/translations/{code}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            IReadOnlyDictionary<string, string>? translations =
                await response.Content.ReadFromJsonAsync<IReadOnlyDictionary<string, string>>();

            translations.Should().NotBeNull();
            translations.Should().NotBeEmpty($"Translations for '{code}' should not be empty");
        }
    }
}