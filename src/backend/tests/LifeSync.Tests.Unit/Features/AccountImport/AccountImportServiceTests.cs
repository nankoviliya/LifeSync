using FluentAssertions;
using LifeSync.API.Features.AccountImport;
using LifeSync.API.Features.AccountImport.DataReaders;
using LifeSync.API.Features.AccountImport.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Languages;
using LifeSync.API.Persistence;
using LifeSync.API.Secrets.Contracts;
using LifeSync.API.Shared;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using LifeSync.UnitTests.SharedUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Data.Common;
using System.Text.Json;

namespace LifeSync.UnitTests.Features.AccountImport;

public class AccountImportServiceTests
{
    private readonly Guid _testLanguageId;
    private readonly string _testUserId;

    private readonly DbConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _contextOptions;

    private readonly IAccountDataReader _dataReader;
    private readonly ILogger<AccountImportService> _logger;
    private readonly ISecretsManager _secretsManager;

    public AccountImportServiceTests()
    {
        _logger = Substitute.For<ILogger<AccountImportService>>();
        _dataReader = Substitute.For<IAccountDataReader>();
        _secretsManager = Substitute.For<ISecretsManager>();

        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        using ApplicationDbContext context = new(_contextOptions, _secretsManager);
        if (context.Database.EnsureCreated())
        {
            Language language = Language.From("English".ToRequiredString(), "en".ToRequiredString());

            context.Add(language);

            _testLanguageId = language.Id;

            User user = User.From(
                "user123@gmail.com".ToRequiredString(),
                "user123@gmail.com".ToRequiredString(),
                "F".ToRequiredString(),
                "L".ToRequiredString(),
                new Money(200, Currency.Bgn).ToRequiredReference(),
                Currency.Bgn.ToRequiredReference(),
                _testLanguageId.ToRequiredStruct()
            );

            context.Add(user);

            _testUserId = user.Id.ToRequiredString();

            context.SaveChanges();
        }
    }

    private ApplicationDbContext CreateContext() => new(_contextOptions, _secretsManager);

    public void Dispose() => _connection.Dispose();

    private (AccountImportRequest request, ImportAccountData testData) CreateValidTestData()
    {
        ImportAccountData testData = ImportData.GetData(_testLanguageId.ToRequiredStruct());

        string json = JsonSerializer.Serialize(testData);
        IFormFile file = ImportData.CreateSubstituteFormFile("test.json", json);

        AccountImportRequest request = new() { Format = AccountImportFileFormat.Json, File = file };

        return (request, testData);
    }

    [Fact]
    public async Task ImportAccountDataAsync_ShouldReturnSuccessMessageResult_WhenImportIsSuccessful()
    {
        (AccountImportRequest request, ImportAccountData testData) = CreateValidTestData();

        AccountImportService accountImportService = new(CreateContext(), [_dataReader], _logger);

        _dataReader.Format.Returns(AccountImportFileFormat.Json);
        _dataReader.ReadAsync(request.File, Arg.Any<CancellationToken>()).Returns(testData);

        MessageResult result =
            await accountImportService.ImportAccountDataAsync(
                _testUserId.ToRequiredString(),
                request,
                CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Account data imported successfully.");

        await using ApplicationDbContext assertContext = CreateContext();
        User? updatedUser = await assertContext.Users.FindAsync(_testUserId);
        updatedUser.Should().NotBeNull();
        updatedUser!.Balance.Amount.Should().Be(testData.ProfileData.BalanceAmount);
        updatedUser.Balance.Currency.Code.Should().Be(testData.ProfileData.BalanceCurrency);
        updatedUser.LanguageId.Should().Be(testData.ProfileData.LanguageId);

        int incomeCount = await assertContext.IncomeTransactions.CountAsync();
        int expenseCount = await assertContext.ExpenseTransactions.CountAsync();
        incomeCount.Should().Be(1);
        expenseCount.Should().Be(1);
    }

    [Fact]
    public async Task ImportAccountDataAsync_ShouldReturnFailure_WhenFormatIsUnsupported()
    {
        (AccountImportRequest request, _) = CreateValidTestData();

        AccountImportService sut = new(CreateContext(), [], _logger);

        MessageResult result =
            await sut.ImportAccountDataAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Unsupported file format.");
    }

    [Fact]
    public async Task ImportAccountDataAsync_ShouldReturnFailure_WhenDataReaderReturnsNull()
    {
        (AccountImportRequest request, _) = CreateValidTestData();

        _dataReader.Format.Returns(AccountImportFileFormat.Json);
        _dataReader.ReadAsync(request.File, Arg.Any<CancellationToken>()).Returns((ImportAccountData?)null);

        AccountImportService sut = new(CreateContext(), [_dataReader], _logger);

        MessageResult result =
            await sut.ImportAccountDataAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Cannot read data from file.");
    }

    [Fact]
    public async Task ImportAccountDataAsync_ShouldReturnFailure_WhenUserIsNotFound()
    {
        (AccountImportRequest request, ImportAccountData testData) = CreateValidTestData();

        _dataReader.Format.Returns(AccountImportFileFormat.Json);
        _dataReader.ReadAsync(request.File, Arg.Any<CancellationToken>()).Returns(testData);

        string nonExistentUserId = Guid.NewGuid().ToString();

        AccountImportService sut = new(CreateContext(), [_dataReader], _logger);

        MessageResult result =
            await sut.ImportAccountDataAsync(nonExistentUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("User account not found.");
    }

    [Fact]
    public async Task ImportAccountDataAsync_ShouldReturnFailureAndRollback_WhenDbUpdateFails()
    {
        (AccountImportRequest request, ImportAccountData testData) = CreateValidTestData();

        _dataReader.Format.Returns(AccountImportFileFormat.Json);
        _dataReader.ReadAsync(request.File, Arg.Any<CancellationToken>()).Returns(testData);

        FailingApplicationDbContext dbContext = new(_contextOptions, _secretsManager);
        dbContext.SetSaveChangesShouldFail(true);

        AccountImportService sut = new(dbContext, [_dataReader], _logger);

        MessageResult result =
            await sut.ImportAccountDataAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Import failed. Please try again.");
        _logger.Received(1).LogError(Arg.Any<DbUpdateException>(), "Transaction failed");

        // Verify that no changes were committed to the actual database
        await using ApplicationDbContext assertContext = CreateContext();
        User? userAfterFailure = await assertContext.Users.FindAsync(_testUserId);
        userAfterFailure!.Balance.Amount.Should().Be(200);
        userAfterFailure.LanguageId.Should().Be(_testLanguageId);

        int incomeCount = await assertContext.IncomeTransactions.CountAsync();
        int expenseCount = await assertContext.ExpenseTransactions.CountAsync();
        incomeCount.Should().Be(0);
        expenseCount.Should().Be(0);
    }
}