using FluentAssertions;
using LifeSync.API.Features.Finances.Incomes.Models;
using LifeSync.API.Features.Finances.Incomes.Services;
using LifeSync.API.Features.Finances.Shared.ResultMessages;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Incomes;
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

namespace LifeSync.UnitTests.Features.Finances.Incomes.Services;

public class IncomeServiceTests
{
    private readonly Guid _testLanguageId;
    private readonly string _testUserId = default!;

    private readonly DbConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _contextOptions;

    private readonly ILogger<IncomeService> _logger;
    private readonly ISecretsManager _secretsManager;

    public IncomeServiceTests()
    {
        _logger = Substitute.For<ILogger<IncomeService>>();
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
                "user@test.com".ToRequiredString(),
                "user@test.com".ToRequiredString(),
                "Jane".ToRequiredString(),
                "Smith".ToRequiredString(),
                new Money(500, "BGN").ToRequiredReference(),
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
    public async Task AddIncomeAsync_ShouldReturnSuccessDataResult_WhenIncomeIsAddedSuccessfully()
    {
        AddIncomeRequest request = new()
        {
            Amount = 1500,
            Currency = "BGN",
            Date = DateTime.UtcNow,
            Description = "Monthly salary"
        };

        IncomeService sut = new(CreateContext(), _logger);

        DataResult<Guid> result = await sut.AddIncomeAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();

        await using ApplicationDbContext assertContext = CreateContext();
        IncomeTransaction? addedIncome = await assertContext.IncomeTransactions.FindAsync(result.Data);
        addedIncome.Should().NotBeNull();
        addedIncome!.Amount.Amount.Should().Be(1500);
        addedIncome.Description.Should().Be("Monthly salary");
    }

    [Fact]
    public async Task AddIncomeAsync_ShouldUpdateUserBalance_WhenIncomeIsAdded()
    {
        AddIncomeRequest request = new()
        {
            Amount = 2000,
            Currency = "BGN",
            Date = DateTime.UtcNow,
            Description = "Bonus"
        };

        IncomeService sut = new(CreateContext(), _logger);

        DataResult<Guid> result = await sut.AddIncomeAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        await using ApplicationDbContext assertContext = CreateContext();
        User? user = await assertContext.Users.FindAsync(_testUserId);
        user.Should().NotBeNull();
        user!.Balance.Amount.Should().Be(2500); // 500 + 2000
    }

    [Fact]
    public async Task AddIncomeAsync_ShouldReturnFailure_WhenUserIsNotFound()
    {
        AddIncomeRequest request = new()
        {
            Amount = 500,
            Currency = "BGN",
            Date = DateTime.UtcNow,
            Description = "Test income"
        };

        string nonExistentUserId = Guid.NewGuid().ToString();

        IncomeService sut = new(CreateContext(), _logger);

        DataResult<Guid> result = await sut.AddIncomeAsync(nonExistentUserId.ToRequiredString(), request, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(FinancesResultMessages.UserNotFound);
    }

    [Fact]
    public async Task AddIncomeAsync_ShouldReturnFailure_WhenCurrencyMismatch()
    {
        AddIncomeRequest request = new()
        {
            Amount = 1000,
            Currency = "EUR",
            Date = DateTime.UtcNow,
            Description = "Freelance payment"
        };

        IncomeService sut = new(CreateContext(), _logger);

        DataResult<Guid> result = await sut.AddIncomeAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(FinancesResultMessages.CurrencyMismatch);

        await using ApplicationDbContext assertContext = CreateContext();
        User? user = await assertContext.Users.FindAsync(_testUserId);
        user!.Balance.Amount.Should().Be(500); // Balance should remain unchanged
    }

    [Fact]
    public async Task AddIncomeAsync_ShouldRollbackTransaction_WhenExceptionOccurs()
    {
        AddIncomeRequest request = new()
        {
            Amount = 800,
            Currency = "BGN",
            Date = DateTime.UtcNow,
            Description = "Test"
        };

        FailingApplicationDbContext dbContext = new(_contextOptions, _secretsManager);
        dbContext.SetSaveChangesShouldFail(true);

        IncomeService sut = new(dbContext, _logger);

        DataResult<Guid> result = await sut.AddIncomeAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(FinancesResultMessages.RequestFailed);

        // Verify no income was added and balance remains unchanged
        await using ApplicationDbContext assertContext = CreateContext();
        int incomeCount = await assertContext.IncomeTransactions.CountAsync();
        incomeCount.Should().Be(0);

        User? user = await assertContext.Users.FindAsync(_testUserId);
        user!.Balance.Amount.Should().Be(500); // Balance should remain unchanged
    }

    [Fact]
    public async Task AddIncomeAsync_ShouldAssociateIncomeWithCorrectUser()
    {
        AddIncomeRequest request = new()
        {
            Amount = 1200,
            Currency = "BGN",
            Date = DateTime.UtcNow,
            Description = "Consulting fee"
        };

        IncomeService sut = new(CreateContext(), _logger);

        DataResult<Guid> result = await sut.AddIncomeAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        await using ApplicationDbContext assertContext = CreateContext();
        IncomeTransaction? addedIncome = await assertContext.IncomeTransactions.FindAsync(result.Data);
        addedIncome.Should().NotBeNull();
        addedIncome!.UserId.Should().Be(_testUserId);
    }

    [Fact]
    public async Task AddIncomeAsync_ShouldCreateIncomeWithCorrectDate()
    {
        DateTime specificDate = new DateTime(2024, 11, 1, 9, 0, 0, DateTimeKind.Utc);

        AddIncomeRequest request = new()
        {
            Amount = 3000,
            Currency = "BGN",
            Date = specificDate,
            Description = "Project payment"
        };

        IncomeService sut = new(CreateContext(), _logger);

        DataResult<Guid> result = await sut.AddIncomeAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        await using ApplicationDbContext assertContext = CreateContext();
        IncomeTransaction? addedIncome = await assertContext.IncomeTransactions.FindAsync(result.Data);
        addedIncome.Should().NotBeNull();
        addedIncome!.Date.Should().Be(specificDate);
    }

    [Fact]
    public async Task AddIncomeAsync_ShouldHandleMultipleIncomesForSameUser()
    {
        AddIncomeRequest request1 = new()
        {
            Amount = 1000,
            Currency = "BGN",
            Date = DateTime.UtcNow,
            Description = "Income 1"
        };

        AddIncomeRequest request2 = new()
        {
            Amount = 500,
            Currency = "BGN",
            Date = DateTime.UtcNow,
            Description = "Income 2"
        };

        IncomeService sut = new(CreateContext(), _logger);

        DataResult<Guid> result1 = await sut.AddIncomeAsync(_testUserId.ToRequiredString(), request1, CancellationToken.None);
        DataResult<Guid> result2 = await sut.AddIncomeAsync(_testUserId.ToRequiredString(), request2, CancellationToken.None);

        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();

        await using ApplicationDbContext assertContext = CreateContext();
        int incomeCount = await assertContext.IncomeTransactions.CountAsync(i => i.UserId == _testUserId);
        incomeCount.Should().Be(2);

        User? user = await assertContext.Users.FindAsync(_testUserId);
        user!.Balance.Amount.Should().Be(2000); // 500 + 1000 + 500
    }
}
