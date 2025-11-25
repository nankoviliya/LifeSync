using FluentAssertions;
using LifeSync.API.Features.Account.GetAccount.Models;
using LifeSync.API.Features.Account.GetAccount.Services;
using LifeSync.API.Features.Account.Shared;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Languages;
using LifeSync.API.Persistence;
using LifeSync.API.Secrets.Contracts;
using LifeSync.API.Shared;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Data.Common;

namespace LifeSync.Tests.Unit.Features.Account.GetAccount.Services;

public class GetAccountServiceTests : IDisposable
{
    private readonly Guid _testLanguageId;
    private readonly string _testUserId;

    private readonly DbConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _contextOptions;

    private readonly ILogger<GetAccountService> _logger;
    private readonly ISecretsManager _secretsManager;

    public GetAccountServiceTests()
    {
        _logger = Substitute.For<ILogger<GetAccountService>>();
        _secretsManager = Substitute.For<ISecretsManager>();

        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        using ApplicationDbContext context = new(_contextOptions, _secretsManager);
        if (context.Database.EnsureCreated())
        {
            Language englishLanguage = Language.From("English".ToRequiredString(), "en".ToRequiredString());

            context.Languages.Add(englishLanguage);

            _testLanguageId = englishLanguage.Id;

            User user = User.From(
                "testuser".ToRequiredString(),
                "test@example.com".ToRequiredString(),
                "John".ToRequiredString(),
                "Doe".ToRequiredString(),
                new Money(1500, "USD").ToRequiredReference(),
                "USD".ToRequiredString(),
                _testLanguageId.ToRequiredStruct()
            );

            context.Users.Add(user);

            _testUserId = user.Id;

            context.SaveChanges();
        }
    }

    private ApplicationDbContext CreateContext() => new(_contextOptions, _secretsManager);

    public void Dispose() => _connection.Dispose();

    [Fact]
    public async Task GetUserAccountDataAsync_ShouldReturnSuccess_WhenUserExists()
    {
        GetAccountService sut = new(CreateContext(), _logger);

        DataResult<GetAccountResponse> result =
            await sut.GetUserAccountDataAsync(_testUserId.ToRequiredString(), CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserAccountDataAsync_ShouldReturnCorrectUserData()
    {
        GetAccountService sut = new(CreateContext(), _logger);

        DataResult<GetAccountResponse> result =
            await sut.GetUserAccountDataAsync(_testUserId.ToRequiredString(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.UserId.Should().Be(_testUserId);
        result.Data.Email.Should().Be("test@example.com");
        result.Data.UserName.Should().Be("testuser");
        result.Data.FirstName.Should().Be("John");
        result.Data.LastName.Should().Be("Doe");
        result.Data.BalanceAmount.Should().Be(1500);
        result.Data.BalanceCurrency.Should().Be("USD");
        result.Data.Language.Should().NotBeNull();
        result.Data.Language.Id.Should().Be(_testLanguageId);
        result.Data.Language.Code.Should().Be("en");
    }

    [Fact]
    public async Task GetUserAccountDataAsync_ShouldReturnFailure_WhenUserNotFound()
    {
        string nonExistentUserId = Guid.NewGuid().ToString();

        GetAccountService sut = new(CreateContext(), _logger);

        DataResult<GetAccountResponse> result =
            await sut.GetUserAccountDataAsync(nonExistentUserId.ToRequiredString(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(AccountResultMessages.UserNotFound);
    }

    [Fact]
    public async Task GetUserAccountDataAsync_ShouldNotTrackEntities()
    {
        ApplicationDbContext context = CreateContext();
        GetAccountService sut = new(context, _logger);

        await sut.GetUserAccountDataAsync(_testUserId.ToRequiredString(), CancellationToken.None);

        context.ChangeTracker.Entries().Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserAccountDataAsync_ShouldLogWarning_WhenUserNotFound()
    {
        string nonExistentUserId = Guid.NewGuid().ToString();

        GetAccountService sut = new(CreateContext(), _logger);

        await sut.GetUserAccountDataAsync(nonExistentUserId.ToRequiredString(), CancellationToken.None);

        _logger.Received(1).Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("User not found")),
            null,
            Arg.Any<Func<object, Exception?, string>>()
        );
    }
}
