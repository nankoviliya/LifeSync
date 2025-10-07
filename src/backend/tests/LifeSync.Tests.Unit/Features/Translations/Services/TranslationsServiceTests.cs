using FluentAssertions;
using LifeSync.API.Features.Translations.ResultMessages;
using LifeSync.API.Features.Translations.Services;
using LifeSync.API.Features.Translations.Services.Contracts;
using LifeSync.API.Models.Languages;
using LifeSync.API.Persistence;
using LifeSync.API.Secrets.Contracts;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Data.Common;

namespace LifeSync.UnitTests.Features.Translations.Services;

public class TranslationsServiceTests
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _contextOptions;

    private readonly ISecretsManager _secretsManager = Substitute.For<ISecretsManager>();

    private readonly ITranslationsLoader _translationsLoader = Substitute.For<ITranslationsLoader>();
    private readonly ILogger<TranslationsService> _logger = Substitute.For<ILogger<TranslationsService>>();

    public TranslationsServiceTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        ApplicationDbContext? context = new ApplicationDbContext(_contextOptions, _secretsManager);

        if (context.Database.EnsureCreated())
        {
            using DbCommand? viewCommand = context.Database.GetDbConnection().CreateCommand();

            viewCommand.ExecuteNonQuery();
        }

        context.Add(Language.From("English".ToRequiredString(), "en".ToRequiredString()));
        context.SaveChanges();
    }

    private ApplicationDbContext CreateContext() => new(_contextOptions, _secretsManager);

    [Fact]
    public async Task GetTranslationsByLanguageCodeAsync_ShouldReturnFailure_WhenLanguageCodeIsNullOrEmpty()
    {
        string? emptyLanguageCode = string.Empty;

        TranslationsService? translationsService =
            new TranslationsService(CreateContext(), _translationsLoader, _logger);

        DataResult<IReadOnlyDictionary<string, string>>? result =
            await translationsService.GetTranslationsByLanguageCodeAsync(emptyLanguageCode, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(TranslationsResultMessages.LanguageCodeNotProvided);
    }

    [Fact]
    public async Task GetTranslationsByLanguageCodeAsync_ShouldReturnFailure_WhenLanguageNotFound()
    {
        string? nonExistingLanguage = "TEST_nOn-EXiSTED-CODE";

        TranslationsService? translationsService =
            new TranslationsService(CreateContext(), _translationsLoader, _logger);

        DataResult<IReadOnlyDictionary<string, string>>? result =
            await translationsService.GetTranslationsByLanguageCodeAsync(nonExistingLanguage, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(TranslationsResultMessages.LanguageNotFound);
    }

    [Fact]
    public async Task GetTranslationsByLanguageCodeAsync_ShouldReturnTranslations_WhenLanguageIsValid()
    {
        string? languageCode = "en";

        TranslationsService? translationsService =
            new TranslationsService(CreateContext(), _translationsLoader, _logger);

        _translationsLoader.LoadTranslationsAsync(languageCode, CancellationToken.None).Returns(
            new Dictionary<string, string> { { "button", "Click" }, { "input-label", "Insert value" } });

        DataResult<IReadOnlyDictionary<string, string>>? result =
            await translationsService.GetTranslationsByLanguageCodeAsync(languageCode, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data["button"].Should().Be("Click");
        result.Data["input-label"].Should().Be("Insert value");
    }

    [Fact]
    public async Task GetTranslationsByLanguageCodeAsync_ShouldReturnTranslationsFromCache_WhenMoreThanOneCall()
    {
        string? languageCode = "en";

        TranslationsService? translationsService =
            new TranslationsService(CreateContext(), _translationsLoader, _logger);

        _translationsLoader.LoadTranslationsAsync(languageCode, CancellationToken.None)
            .Returns(new Dictionary<string, string> { { "button", "Click" } });

        DataResult<IReadOnlyDictionary<string, string>>? result1 =
            await translationsService.GetTranslationsByLanguageCodeAsync(languageCode, CancellationToken.None);
        DataResult<IReadOnlyDictionary<string, string>>? result2 =
            await translationsService.GetTranslationsByLanguageCodeAsync(languageCode, CancellationToken.None);

        await _translationsLoader.Received(1).LoadTranslationsAsync(languageCode, CancellationToken.None);
        result1.Data.Should().BeEquivalentTo(result2.Data);
    }

    [Fact]
    public async Task GetTranslationsByLanguageCodeAsync_LanguageCodeCheckShouldBeCaseInsensitive()
    {
        string? languageCode = "EN";

        TranslationsService? translationsService =
            new TranslationsService(CreateContext(), _translationsLoader, _logger);

        _translationsLoader.LoadTranslationsAsync(languageCode, CancellationToken.None)
            .Returns(new Dictionary<string, string> { { "button", "Click" } });

        DataResult<IReadOnlyDictionary<string, string>>? result =
            await translationsService.GetTranslationsByLanguageCodeAsync(languageCode, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data["button"].Should().Be("Click");
    }
}