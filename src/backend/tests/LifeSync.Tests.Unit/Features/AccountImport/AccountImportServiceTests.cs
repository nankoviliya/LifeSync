using LifeSync.API.Features.AccountImport;
using LifeSync.API.Features.AccountImport.DataReaders;
using LifeSync.API.Persistence;
using LifeSync.API.Secrets.Contracts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Data.Common;

namespace LifeSync.UnitTests.Features.AccountImport;

public class AccountImportServiceTests
{
    private const string TestUserId = "DD75088E-F024-4F5F-9A41-696F64639096";

    private readonly DbConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _contextOptions;
    private readonly ISecretsManager _secretsManager;

    private readonly ILogger<AccountImportService> _logger;
    private readonly ApplicationDbContext _databaseContext;
    private readonly IAccountDataReader _dataReader;

    public AccountImportServiceTests()
    {
        _secretsManager = Substitute.For<ISecretsManager>();

        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        ApplicationDbContext? context = new(_contextOptions, _secretsManager);

        if (context.Database.EnsureCreated())
        {
            using DbCommand? viewCommand = context.Database.GetDbConnection().CreateCommand();

            viewCommand.ExecuteNonQuery();
        }

        _logger = Substitute.For<ILogger<AccountImportService>>();
        _databaseContext = Substitute.For<ApplicationDbContext>();
        _dataReader = Substitute.For<IAccountDataReader>();
    }

    // TODO: Implement importer tests

    // [Fact]
    // public async Task ImportAccountDataAsync_ShouldReturnSuccessMessageResult_WhenImportIsSuccessful()
    // {
    //     AccountImportService accountImportService = new(_databaseContext, [_dataReader], _logger);
    //
    //     ImportAccountData expectedData = ImportData.Data;
    //
    //     string json = JsonSerializer.Serialize(expectedData);
    //     IFormFile file = ImportData.CreateSubstituteFormFile("test.json", json);
    //
    //     AccountImportRequest request = new AccountImportRequest { Format = AccountImportFileFormat.Json, File = file };
    //
    //     User user = new User
    //     {
    //         Id = TestUserId, Balance = new Money(0, Currency.FromCode("BGN")), LanguageId = Guid.NewGuid()
    //     };
    //
    //     _dataReader.Format.Returns(AccountImportFileFormat.Json);
    //     _dataReader.ReadAsync(file, Arg.Any<CancellationToken>()).Returns(expectedData);
    //
    //     _databaseContext.Users.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
    //         .Returns(user);
    //
    //     _databaseContext.Database.BeginTransactionAsync(Arg.Any<CancellationToken>())
    //         .Returns(_transaction);
    //
    //     _databaseContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
    //
    //     MessageResult result =
    //         await accountImportService.ImportAccountDataAsync(TestUserId, request, CancellationToken.None);
    //
    //     result.Should().NotBeNull();
    //     result.IsSuccess.Should().BeTrue();
    //     result.Message.Should().Be("Account data imported successfully.");
    //
    //     await _databaseContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    //     await _transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    //     await _transaction.DidNotReceive().RollbackAsync(Arg.Any<CancellationToken>());
    // }
}