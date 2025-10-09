using FluentAssertions;
using LifeSync.API.Features.AccountExport;
using LifeSync.API.Features.AccountExport.DataExporters;
using LifeSync.API.Features.AccountExport.Models;
using LifeSync.API.Features.AccountExport.ResultMessages;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Expenses;
using LifeSync.API.Models.Incomes;
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

namespace LifeSync.UnitTests.Features.AccountExport;

public class AccountExportServiceTests
{
    private readonly Guid _testLanguageId;
    private readonly string _testUserId;

    private readonly DbConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _contextOptions;

    private readonly IAccountDataExporter _dataExporter;
    private readonly ILogger<AccountExportService> _logger;
    private readonly ISecretsManager _secretsManager;

    public AccountExportServiceTests()
    {
        _logger = Substitute.For<ILogger<AccountExportService>>();
        _dataExporter = Substitute.For<IAccountDataExporter>();
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
                new Money(200, "BGN").ToRequiredReference(),
                "BGN".ToRequiredString(),
                _testLanguageId.ToRequiredStruct()
            );

            context.Add(user);

            _testUserId = user.Id.ToRequiredString();

            context.SaveChanges();
        }
    }

    private ApplicationDbContext CreateContext() => new(_contextOptions, _secretsManager);

    public void Dispose() => _connection.Dispose();

    [Fact]
    public async Task ExportAccountData_ShouldReturnSuccessDataResult_WhenExportIsSuccessful()
    {
        ExportAccountRequest request = new() { Format = ExportAccountFileFormat.Json };

        ExportAccountResponse expectedResponse = new()
        {
            Data = new byte[] { 1, 2, 3 }, ContentType = "application/json", FileName = "account-data.json"
        };

        _dataExporter.Format.Returns(ExportAccountFileFormat.Json);
        _dataExporter.Export(Arg.Any<ExportAccountData>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        AccountExportService sut = new(CreateContext(), [_dataExporter], _logger);

        DataResult<ExportAccountResponse> result =
            await sut.ExportAccountData(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Data.Should().BeEquivalentTo(expectedResponse.Data);
        result.Data.ContentType.Should().Be(expectedResponse.ContentType);
        result.Data.FileName.Should().Be(expectedResponse.FileName);
    }

    [Fact]
    public async Task ExportAccountData_ShouldReturnFailure_WhenFormatIsUnsupported()
    {
        ExportAccountRequest request = new() { Format = ExportAccountFileFormat.Json };

        AccountExportService sut = new(CreateContext(), [], _logger);

        DataResult<ExportAccountResponse> result =
            await sut.ExportAccountData(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(AccountExportResultMessages.ExportAccountDataFileFormatInvalid);
    }

    [Fact]
    public async Task ExportAccountData_ShouldReturnFailure_WhenUserIsNotFound()
    {
        ExportAccountRequest request = new() { Format = ExportAccountFileFormat.Json };

        string nonExistentUserId = Guid.NewGuid().ToString();

        _dataExporter.Format.Returns(ExportAccountFileFormat.Json);

        AccountExportService sut = new(CreateContext(), [_dataExporter], _logger);

        DataResult<ExportAccountResponse> result =
            await sut.ExportAccountData(nonExistentUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(AccountExportResultMessages.UserNotFound);
    }

    [Fact]
    public async Task ExportAccountData_ShouldIncludeProfileData_InExportedData()
    {
        ExportAccountRequest request = new() { Format = ExportAccountFileFormat.Json };

        ExportAccountData? capturedAccountData = null;

        _dataExporter.Format.Returns(ExportAccountFileFormat.Json);
        _dataExporter.Export(Arg.Do<ExportAccountData>(data => capturedAccountData = data),
                Arg.Any<CancellationToken>())
            .Returns(new ExportAccountResponse());

        AccountExportService sut = new(CreateContext(), [_dataExporter], _logger);

        await sut.ExportAccountData(_testUserId.ToRequiredString(), request, CancellationToken.None);

        capturedAccountData.Should().NotBeNull();
        capturedAccountData!.ProfileData.Should().NotBeNull();
        capturedAccountData.ProfileData.UserId.Should().Be(_testUserId);
        capturedAccountData.ProfileData.Email.Should().Be("user123@gmail.com");
        capturedAccountData.ProfileData.FirstName.Should().Be("F");
        capturedAccountData.ProfileData.LastName.Should().Be("L");
        capturedAccountData.ProfileData.BalanceAmount.Should().Be(200);
        capturedAccountData.ProfileData.BalanceCurrency.Should().Be("BGN");
        capturedAccountData.ProfileData.LanguageId.Should().Be(_testLanguageId);
        capturedAccountData.ProfileData.LanguageCode.Should().Be("en");
    }

    [Fact]
    public async Task ExportAccountData_ShouldIncludeTransactions_InExportedData()
    {
        await using ApplicationDbContext seedContext = CreateContext();
        
        IncomeTransaction incomeTransaction = IncomeTransaction.From(
            new Money(500, "BGN").ToRequiredReference(),
            DateTime.UtcNow.ToRequiredStruct(),
            "Salary".ToRequiredString(),
            _testUserId.ToRequiredString()
        );

        ExpenseTransaction expenseTransaction = ExpenseTransaction.From(
            new Money(100, "BGN").ToRequiredReference(),
            DateTime.UtcNow.ToRequiredStruct(),
            "Groceries".ToRequiredString(),
            ExpenseType.Needs,
            _testUserId.ToRequiredString()
        );

        seedContext.IncomeTransactions.Add(incomeTransaction);
        seedContext.ExpenseTransactions.Add(expenseTransaction);
        await seedContext.SaveChangesAsync();

        ExportAccountRequest request = new() { Format = ExportAccountFileFormat.Json };

        ExportAccountData? capturedAccountData = null;

        _dataExporter.Format.Returns(ExportAccountFileFormat.Json);
        _dataExporter.Export(Arg.Do<ExportAccountData>(data => capturedAccountData = data),
                Arg.Any<CancellationToken>())
            .Returns(new ExportAccountResponse());

        AccountExportService sut = new(CreateContext(), [_dataExporter], _logger);

        await sut.ExportAccountData(_testUserId.ToRequiredString(), request, CancellationToken.None);

        capturedAccountData.Should().NotBeNull();
        capturedAccountData!.IncomeTransactions.Should().HaveCount(1);
        capturedAccountData.IncomeTransactions[0].Amount.Should().Be(500);
        capturedAccountData.IncomeTransactions[0].Currency.Should().Be("BGN");
        capturedAccountData.IncomeTransactions[0].Description.Should().Be("Salary");

        capturedAccountData.ExpenseTransactions.Should().HaveCount(1);
        capturedAccountData.ExpenseTransactions[0].Amount.Should().Be(100);
        capturedAccountData.ExpenseTransactions[0].Currency.Should().Be("BGN");
        capturedAccountData.ExpenseTransactions[0].Description.Should().Be("Groceries");
        capturedAccountData.ExpenseTransactions[0].ExpenseType.Should().Be(ExpenseType.Needs);
    }

    [Fact]
    public async Task ExportAccountData_ShouldNotTrackEntities()
    {
        ExportAccountRequest request = new() { Format = ExportAccountFileFormat.Json };

        _dataExporter.Format.Returns(ExportAccountFileFormat.Json);
        _dataExporter.Export(Arg.Any<ExportAccountData>(), Arg.Any<CancellationToken>())
            .Returns(new ExportAccountResponse());

        ApplicationDbContext context = CreateContext();
        AccountExportService sut = new(context, [_dataExporter], _logger);

        await sut.ExportAccountData(_testUserId.ToRequiredString(), request, CancellationToken.None);

        context.ChangeTracker.Entries().Should().BeEmpty();
    }
}