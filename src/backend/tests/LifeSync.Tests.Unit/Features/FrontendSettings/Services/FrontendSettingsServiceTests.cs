using FluentAssertions;
using LifeSync.API.Features.FrontendSettings.Models;
using LifeSync.API.Features.FrontendSettings.Services;
using LifeSync.API.Models.Currencies;
using LifeSync.API.Models.Languages;
using LifeSync.API.Persistence;
using LifeSync.API.Secrets.Contracts;
using LifeSync.Common.Results;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System.Data.Common;

namespace LifeSync.UnitTests.Features.FrontendSettings.Services;

public class FrontendSettingsServiceTests
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _contextOptions;
    private readonly ISecretsManager _secretsManager;

    public FrontendSettingsServiceTests()
    {
        _secretsManager = Substitute.For<ISecretsManager>();

        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        using ApplicationDbContext context = new(_contextOptions, _secretsManager);
        if (context.Database.EnsureCreated())
        {
            Language englishLanguage = new() { Name = "English", Code = "en" };
            Language spanishLanguage = new() { Name = "Spanish", Code = "es" };

            context.Languages.AddRange(englishLanguage, spanishLanguage);

            Currency usdCurrency = new()
            {
                Code = "USD",
                Name = "US Dollar",
                NativeName = "US Dollar",
                Symbol = "$",
                NumericCode = 840
            };

            Currency eurCurrency = new()
            {
                Code = "EUR",
                Name = "Euro",
                NativeName = "Euro",
                Symbol = "€",
                NumericCode = 978
            };

            Currency bgnCurrency = new()
            {
                Code = "BGN",
                Name = "Bulgarian Lev",
                NativeName = "Лев",
                Symbol = "лв",
                NumericCode = 975
            };

            context.Currencies.AddRange(usdCurrency, eurCurrency, bgnCurrency);

            context.SaveChanges();
        }
    }

    private ApplicationDbContext CreateContext() => new(_contextOptions, _secretsManager);

    public void Dispose() => _connection.Dispose();

    [Fact]
    public async Task GetFrontendSettingsAsync_ShouldReturnSuccess_WhenDataExists()
    {
        FrontendSettingsService sut = new(CreateContext());

        DataResult<FrontendSettingsResponse> result = await sut.GetFrontendSettingsAsync(CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetFrontendSettingsAsync_ShouldReturnAllLanguageOptions()
    {
        FrontendSettingsService sut = new(CreateContext());

        DataResult<FrontendSettingsResponse> result = await sut.GetFrontendSettingsAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.LanguageOptions.Should().HaveCount(2);
        result.Data.LanguageOptions.Should().Contain(l => l.Name == "English");
        result.Data.LanguageOptions.Should().Contain(l => l.Name == "Spanish");
    }

    [Fact]
    public async Task GetFrontendSettingsAsync_ShouldReturnAllCurrencyOptions()
    {
        FrontendSettingsService sut = new(CreateContext());

        DataResult<FrontendSettingsResponse> result = await sut.GetFrontendSettingsAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.CurrencyOptions.Should().HaveCount(3);
        result.Data.CurrencyOptions.Should().Contain(c => c.Code == "USD" && c.Name == "US Dollar (US Dollar)");
        result.Data.CurrencyOptions.Should().Contain(c => c.Code == "EUR" && c.Name == "Euro (Euro)");
        result.Data.CurrencyOptions.Should().Contain(c => c.Code == "BGN" && c.Name == "Bulgarian Lev (Лев)");
    }

    [Fact]
    public async Task GetFrontendSettingsAsync_ShouldReturnLanguageOptionsWithCorrectIds()
    {
        await using ApplicationDbContext setupContext = CreateContext();
        Language? englishLanguage = await setupContext.Languages.FirstAsync(l => l.Code == "en");

        FrontendSettingsService sut = new(CreateContext());

        DataResult<FrontendSettingsResponse> result = await sut.GetFrontendSettingsAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.LanguageOptions.Should().Contain(l => l.Id == englishLanguage.Id && l.Name == "English");
    }

    [Fact]
    public async Task GetFrontendSettingsAsync_ShouldReturnEmptyLists_WhenNoDataExists()
    {
        await using ApplicationDbContext clearContext = CreateContext();
        clearContext.Languages.RemoveRange(clearContext.Languages);
        clearContext.Currencies.RemoveRange(clearContext.Currencies);
        await clearContext.SaveChangesAsync();

        FrontendSettingsService sut = new(CreateContext());

        DataResult<FrontendSettingsResponse> result = await sut.GetFrontendSettingsAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.LanguageOptions.Should().BeEmpty();
        result.Data.CurrencyOptions.Should().BeEmpty();
    }

    [Fact]
    public async Task GetFrontendSettingsAsync_ShouldFormatCurrencyNameCorrectly()
    {
        FrontendSettingsService sut = new(CreateContext());

        DataResult<FrontendSettingsResponse> result = await sut.GetFrontendSettingsAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        CurrencyOption? bgnCurrency = result.Data.CurrencyOptions.FirstOrDefault(c => c.Code == "BGN");
        bgnCurrency.Should().NotBeNull();
        bgnCurrency!.Name.Should().Be("Bulgarian Lev (Лев)");
    }

    [Fact]
    public async Task GetFrontendSettingsAsync_ShouldNotTrackEntities()
    {
        await using ApplicationDbContext context = CreateContext();
        FrontendSettingsService sut = new(context);

        DataResult<FrontendSettingsResponse> result = await sut.GetFrontendSettingsAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        // Verify no entities are being tracked
        context.ChangeTracker.Entries().Should().BeEmpty();
    }
}
