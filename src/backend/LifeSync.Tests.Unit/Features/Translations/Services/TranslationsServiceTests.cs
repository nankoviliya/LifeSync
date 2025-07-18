using FluentAssertions;
using LifeSync.API.Features.Translations.ResultMessages;
using LifeSync.API.Features.Translations.Services;
using LifeSync.API.Features.Translations.Services.Contracts;
using LifeSync.API.Models.Languages;
using LifeSync.API.Persistence;
using LifeSync.API.Secrets.Contracts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Data.Common;

namespace LifeSync.UnitTests.Features.Translations.Services
{
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

            var context = new ApplicationDbContext(_contextOptions, _secretsManager);

            if (context.Database.EnsureCreated())
            {
                using var viewCommand = context.Database.GetDbConnection().CreateCommand();

                viewCommand.ExecuteNonQuery();
            }

            context.Add(new Language { Name = "English", Code = "en" });
            context.SaveChanges();
        }

        ApplicationDbContext CreateContext() => new ApplicationDbContext(_contextOptions, _secretsManager);

        [Fact]
        public async Task GetTranslationsByLanguageCodeAsync_ShouldReturnFailure_WhenLanguageCodeIsNullOrEmpty()
        {
            var emptyLanguageCode = string.Empty;

            var translationsService = new TranslationsService(CreateContext(), _translationsLoader, _logger);

            var result = await translationsService.GetTranslationsByLanguageCodeAsync(emptyLanguageCode, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(TranslationsResultMessages.LanguageCodeNotProvided);
        }

        [Fact]
        public async Task GetTranslationsByLanguageCodeAsync_ShouldReturnFailure_WhenLanguageNotFound()
        {
            var nonExistingLanguage = "TEST_nOn-EXiSTED-CODE";

            var translationsService = new TranslationsService(CreateContext(), _translationsLoader, _logger);

            var result = await translationsService.GetTranslationsByLanguageCodeAsync(nonExistingLanguage, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(TranslationsResultMessages.LanguageNotFound);
        }

        [Fact]
        public async Task GetTranslationsByLanguageCodeAsync_ShouldReturnTranslations_WhenLanguageIsValid()
        {
            var languageCode = "en";

            var translationsService = new TranslationsService(CreateContext(), _translationsLoader, _logger);

            _translationsLoader.LoadTranslationsAsync(languageCode, CancellationToken.None).Returns(new Dictionary<string, string>
            {
                { "button", "Click" },
                { "input-label", "Insert value" }
            });

            var result = await translationsService.GetTranslationsByLanguageCodeAsync(languageCode, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Data["button"].Should().Be("Click");
            result.Data["input-label"].Should().Be("Insert value");
        }

        [Fact]
        public async Task GetTranslationsByLanguageCodeAsync_ShouldReturnTranslationsFromCache_WhenMoreThanOneCall()
        {
            var languageCode = "en";

            var translationsService = new TranslationsService(CreateContext(), _translationsLoader, _logger);

            _translationsLoader.LoadTranslationsAsync(languageCode, CancellationToken.None).Returns(new Dictionary<string, string>
            {
                { "button", "Click" },
            });

            var result1 = await translationsService.GetTranslationsByLanguageCodeAsync(languageCode, CancellationToken.None);
            var result2 = await translationsService.GetTranslationsByLanguageCodeAsync(languageCode, CancellationToken.None);

            await _translationsLoader.Received(1).LoadTranslationsAsync(languageCode, CancellationToken.None);
            result1.Data.Should().BeEquivalentTo(result2.Data);
        }

        [Fact]
        public async Task GetTranslationsByLanguageCodeAsync_LanguageCodeCheckShouldBeCaseInsensitive()
        {
            var languageCode = "EN";

            var translationsService = new TranslationsService(CreateContext(), _translationsLoader, _logger);

            _translationsLoader.LoadTranslationsAsync(languageCode, CancellationToken.None).Returns(new Dictionary<string, string>
            {
                { "button", "Click" },
            });

            var result = await translationsService.GetTranslationsByLanguageCodeAsync(languageCode, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(1);
            result.Data["button"].Should().Be("Click");
        }
    }
}
