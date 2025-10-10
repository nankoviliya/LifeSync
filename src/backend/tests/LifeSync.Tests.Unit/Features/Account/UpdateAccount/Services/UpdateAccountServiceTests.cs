using FluentAssertions;
using LifeSync.API.Features.Account.Shared;
using LifeSync.API.Features.Account.UpdateAccount.Models;
using LifeSync.API.Features.Account.UpdateAccount.Services;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Languages;
using LifeSync.API.Persistence;
using LifeSync.API.Secrets.Contracts;
using LifeSync.API.Shared;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using LifeSync.UnitTests.SharedUtils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Data.Common;

namespace LifeSync.UnitTests.Features.Account.UpdateAccount.Services;

public class UpdateAccountServiceTests : IDisposable
{
    private readonly Guid _testLanguageId;
    private readonly Guid _testLanguageId2;
    private readonly string _testUserId;

    private readonly DbConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _contextOptions;

    private readonly ILogger<UpdateAccountService> _logger;
    private readonly ISecretsManager _secretsManager;

    public UpdateAccountServiceTests()
    {
        _logger = Substitute.For<ILogger<UpdateAccountService>>();
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
            Language spanishLanguage = Language.From("Spanish".ToRequiredString(), "es".ToRequiredString());

            context.Languages.AddRange(englishLanguage, spanishLanguage);

            _testLanguageId = englishLanguage.Id;
            _testLanguageId2 = spanishLanguage.Id;

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
    public async Task ModifyUserAccountDataAsync_ShouldReturnSuccess_WhenUpdateIsSuccessful()
    {
        UpdateAccountRequest request = new()
        {
            FirstName = "Jane",
            LastName = "Smith",
            LanguageId = _testLanguageId2.ToString()
        };

        UpdateAccountService sut = new(CreateContext(), _logger);

        MessageResult result =
            await sut.ModifyUserAccountDataAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(AccountResultMessages.UserProfileUpdated);
    }

    [Fact]
    public async Task ModifyUserAccountDataAsync_ShouldUpdateUserData_InDatabase()
    {
        UpdateAccountRequest request = new()
        {
            FirstName = "Jane",
            LastName = "Smith",
            LanguageId = _testLanguageId2.ToString()
        };

        UpdateAccountService sut = new(CreateContext(), _logger);

        await sut.ModifyUserAccountDataAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        await using ApplicationDbContext verifyContext = CreateContext();
        User? updatedUser = await verifyContext.Users.FindAsync(_testUserId);

        updatedUser.Should().NotBeNull();
        updatedUser!.FirstName.Should().Be("Jane");
        updatedUser.LastName.Should().Be("Smith");
        updatedUser.LanguageId.Should().Be(_testLanguageId2);
    }

    [Fact]
    public async Task ModifyUserAccountDataAsync_ShouldReturnFailure_WhenUserNotFound()
    {
        string nonExistentUserId = Guid.NewGuid().ToString();

        UpdateAccountRequest request = new()
        {
            FirstName = "Jane",
            LastName = "Smith",
            LanguageId = _testLanguageId.ToString()
        };

        UpdateAccountService sut = new(CreateContext(), _logger);

        MessageResult result =
            await sut.ModifyUserAccountDataAsync(nonExistentUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(AccountResultMessages.UserNotFound);
    }

    [Fact]
    public async Task ModifyUserAccountDataAsync_ShouldReturnFailure_WhenLanguageIdIsInvalid()
    {
        UpdateAccountRequest request = new()
        {
            FirstName = "Jane",
            LastName = "Smith",
            LanguageId = "invalid-guid"
        };

        UpdateAccountService sut = new(CreateContext(), _logger);

        MessageResult result =
            await sut.ModifyUserAccountDataAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(AccountResultMessages.UnableToParseLanguageId);
    }

    [Fact]
    public async Task ModifyUserAccountDataAsync_ShouldLogWarning_WhenUserNotFound()
    {
        string nonExistentUserId = Guid.NewGuid().ToString();

        UpdateAccountRequest request = new()
        {
            FirstName = "Jane",
            LastName = "Smith",
            LanguageId = _testLanguageId.ToString()
        };

        UpdateAccountService sut = new(CreateContext(), _logger);

        await sut.ModifyUserAccountDataAsync(nonExistentUserId.ToRequiredString(), request, CancellationToken.None);

        _logger.Received(1).Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("User not found")),
            null,
            Arg.Any<Func<object, Exception?, string>>()
        );
    }

    [Fact]
    public async Task ModifyUserAccountDataAsync_ShouldLogWarning_WhenLanguageIdIsInvalid()
    {
        UpdateAccountRequest request = new()
        {
            FirstName = "Jane",
            LastName = "Smith",
            LanguageId = "invalid-guid"
        };

        UpdateAccountService sut = new(CreateContext(), _logger);

        await sut.ModifyUserAccountDataAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        _logger.Received(1).Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Unable to parse LanguageId")),
            null,
            Arg.Any<Func<object, Exception?, string>>()
        );
    }

    [Fact]
    public async Task ModifyUserAccountDataAsync_ShouldNotUpdateUserData_WhenUserNotFound()
    {
        string nonExistentUserId = Guid.NewGuid().ToString();

        UpdateAccountRequest request = new()
        {
            FirstName = "Jane",
            LastName = "Smith",
            LanguageId = _testLanguageId.ToString()
        };

        UpdateAccountService sut = new(CreateContext(), _logger);

        await sut.ModifyUserAccountDataAsync(nonExistentUserId.ToRequiredString(), request, CancellationToken.None);

        await using ApplicationDbContext verifyContext = CreateContext();
        User? existingUser = await verifyContext.Users.FindAsync(_testUserId);

        // Original user should remain unchanged
        existingUser.Should().NotBeNull();
        existingUser!.FirstName.Should().Be("John");
        existingUser.LastName.Should().Be("Doe");
        existingUser.LanguageId.Should().Be(_testLanguageId);
    }

    [Fact]
    public async Task ModifyUserAccountDataAsync_ShouldNotUpdateUserData_WhenLanguageIdIsInvalid()
    {
        UpdateAccountRequest request = new()
        {
            FirstName = "Jane",
            LastName = "Smith",
            LanguageId = "invalid-guid"
        };

        UpdateAccountService sut = new(CreateContext(), _logger);

        await sut.ModifyUserAccountDataAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        await using ApplicationDbContext verifyContext = CreateContext();
        User? existingUser = await verifyContext.Users.FindAsync(_testUserId);

        // User should remain unchanged
        existingUser.Should().NotBeNull();
        existingUser!.FirstName.Should().Be("John");
        existingUser.LastName.Should().Be("Doe");
        existingUser.LanguageId.Should().Be(_testLanguageId);
    }

    [Fact]
    public async Task ModifyUserAccountDataAsync_ShouldOnlyUpdateProvidedFields()
    {
        // First, verify the initial state
        await using ApplicationDbContext initialContext = CreateContext();
        User? initialUser = await initialContext.Users.AsNoTracking().FirstAsync(u => u.Id == _testUserId);
        decimal initialBalance = initialUser.Balance.Amount;
        string initialCurrency = initialUser.Balance.CurrencyCode;

        UpdateAccountRequest request = new()
        {
            FirstName = "UpdatedFirst",
            LastName = "UpdatedLast",
            LanguageId = _testLanguageId2.ToString()
        };

        UpdateAccountService sut = new(CreateContext(), _logger);

        await sut.ModifyUserAccountDataAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        await using ApplicationDbContext verifyContext = CreateContext();
        User? updatedUser = await verifyContext.Users.FindAsync(_testUserId);

        updatedUser.Should().NotBeNull();
        updatedUser!.FirstName.Should().Be("UpdatedFirst");
        updatedUser.LastName.Should().Be("UpdatedLast");
        updatedUser.LanguageId.Should().Be(_testLanguageId2);

        // Balance and other fields should not change
        updatedUser.Balance.Amount.Should().Be(initialBalance);
        updatedUser.Balance.CurrencyCode.Should().Be(initialCurrency);
        updatedUser.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task ModifyUserAccountDataAsync_ShouldLogError_WhenExceptionOccurs()
    {
        UpdateAccountRequest request = new()
        {
            FirstName = "Jane",
            LastName = "Smith",
            LanguageId = _testLanguageId.ToString()
        };

        // Use FailingApplicationDbContext to simulate database error
        FailingApplicationDbContext dbContext = new(_contextOptions, _secretsManager);
        dbContext.SetSaveChangesShouldFail(true);

        UpdateAccountService sut = new(dbContext, _logger);

        MessageResult result = await sut.ModifyUserAccountDataAsync(
            _testUserId.ToRequiredString(),
            request,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("An error occurred while updating the user profile.");

        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>()
        );
    }
}
