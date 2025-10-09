using FluentAssertions;
using LifeSync.API.Features.Finances.Expenses.Models;
using LifeSync.API.Features.Finances.Expenses.Services;
using LifeSync.API.Features.Finances.Shared.ResultMessages;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Expenses;
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

namespace LifeSync.UnitTests.Features.Finances.Expenses.Services;

public class ExpenseServiceTests
{
    private readonly Guid _testLanguageId;
    private readonly string _testUserId = default!;

    private readonly DbConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _contextOptions;

    private readonly ILogger<ExpenseService> _logger;
    private readonly ISecretsManager _secretsManager;

    public ExpenseServiceTests()
    {
        _logger = Substitute.For<ILogger<ExpenseService>>();
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
                "John".ToRequiredString(),
                "Doe".ToRequiredString(),
                new Money(1000, "BGN").ToRequiredReference(),
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
    public async Task AddExpenseAsync_ShouldReturnSuccessDataResult_WhenExpenseIsAddedSuccessfully()
    {
        AddExpenseRequest request = new()
        {
            Amount = 100,
            Currency = "BGN",
            Date = DateTime.UtcNow,
            Description = "Groceries",
            ExpenseType = ExpenseType.Needs
        };

        ExpenseService sut = new(CreateContext(), _logger);

        DataResult<Guid> result = await sut.AddExpenseAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();

        await using ApplicationDbContext assertContext = CreateContext();
        ExpenseTransaction? addedExpense = await assertContext.ExpenseTransactions.FindAsync(result.Data);
        addedExpense.Should().NotBeNull();
        addedExpense!.Amount.Amount.Should().Be(100);
        addedExpense.Description.Should().Be("Groceries");
        addedExpense.ExpenseType.Should().Be(ExpenseType.Needs);
    }

    [Fact]
    public async Task AddExpenseAsync_ShouldUpdateUserBalance_WhenExpenseIsAdded()
    {
        AddExpenseRequest request = new()
        {
            Amount = 100,
            Currency = "BGN",
            Date = DateTime.UtcNow,
            Description = "Shopping",
            ExpenseType = ExpenseType.Wants
        };

        ExpenseService sut = new(CreateContext(), _logger);

        DataResult<Guid> result = await sut.AddExpenseAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        await using ApplicationDbContext assertContext = CreateContext();
        User? user = await assertContext.Users.FindAsync(_testUserId);
        user.Should().NotBeNull();
        user!.Balance.Amount.Should().Be(900); // 1000 - 100
    }

    [Fact]
    public async Task AddExpenseAsync_ShouldReturnFailure_WhenUserIsNotFound()
    {
        AddExpenseRequest request = new()
        {
            Amount = 50,
            Currency = "BGN",
            Date = DateTime.UtcNow,
            Description = "Test",
            ExpenseType = ExpenseType.Needs
        };

        string nonExistentUserId = Guid.NewGuid().ToString();

        ExpenseService sut = new(CreateContext(), _logger);

        DataResult<Guid> result = await sut.AddExpenseAsync(nonExistentUserId.ToRequiredString(), request, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(FinancesResultMessages.UserNotFound);
    }

    [Fact]
    public async Task AddExpenseAsync_ShouldReturnFailure_WhenCurrencyMismatch()
    {
        AddExpenseRequest request = new()
        {
            Amount = 100,
            Currency = "USD",
            Date = DateTime.UtcNow,
            Description = "Test",
            ExpenseType = ExpenseType.Needs
        };

        ExpenseService sut = new(CreateContext(), _logger);

        DataResult<Guid> result = await sut.AddExpenseAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(FinancesResultMessages.CurrencyMismatch);

        await using ApplicationDbContext assertContext = CreateContext();
        User? user = await assertContext.Users.FindAsync(_testUserId);
        user!.Balance.Amount.Should().Be(1000); // Balance should remain unchanged
    }

    [Fact]
    public async Task AddExpenseAsync_ShouldCreateExpenseWithCorrectExpenseType()
    {
        AddExpenseRequest savingsRequest = new()
        {
            Amount = 200,
            Currency = "BGN",
            Date = DateTime.UtcNow,
            Description = "Investment",
            ExpenseType = ExpenseType.Savings
        };

        ExpenseService sut = new(CreateContext(), _logger);

        DataResult<Guid> result = await sut.AddExpenseAsync(_testUserId.ToRequiredString(), savingsRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        await using ApplicationDbContext assertContext = CreateContext();
        ExpenseTransaction? addedExpense = await assertContext.ExpenseTransactions.FindAsync(result.Data);
        addedExpense.Should().NotBeNull();
        addedExpense!.ExpenseType.Should().Be(ExpenseType.Savings);
    }

    [Fact]
    public async Task AddExpenseAsync_ShouldRollbackTransaction_WhenExceptionOccurs()
    {
        AddExpenseRequest request = new()
        {
            Amount = 100,
            Currency = "BGN",
            Date = DateTime.UtcNow,
            Description = "Test",
            ExpenseType = ExpenseType.Needs
        };

        FailingApplicationDbContext dbContext = new(_contextOptions, _secretsManager);
        dbContext.SetSaveChangesShouldFail(true);

        ExpenseService sut = new(dbContext, _logger);

        DataResult<Guid> result = await sut.AddExpenseAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(FinancesResultMessages.RequestFailed);

        // Verify no expense was added and balance remains unchanged
        await using ApplicationDbContext assertContext = CreateContext();
        int expenseCount = await assertContext.ExpenseTransactions.CountAsync();
        expenseCount.Should().Be(0);

        User? user = await assertContext.Users.FindAsync(_testUserId);
        user!.Balance.Amount.Should().Be(1000); // Balance should remain unchanged
    }

    [Fact]
    public async Task AddExpenseAsync_ShouldAssociateExpenseWithCorrectUser()
    {
        AddExpenseRequest request = new()
        {
            Amount = 75,
            Currency = "BGN",
            Date = DateTime.UtcNow,
            Description = "Utilities",
            ExpenseType = ExpenseType.Needs
        };

        ExpenseService sut = new(CreateContext(), _logger);

        DataResult<Guid> result = await sut.AddExpenseAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        await using ApplicationDbContext assertContext = CreateContext();
        ExpenseTransaction? addedExpense = await assertContext.ExpenseTransactions.FindAsync(result.Data);
        addedExpense.Should().NotBeNull();
        addedExpense!.UserId.Should().Be(_testUserId);
    }

    [Fact]
    public async Task AddExpenseAsync_ShouldCreateExpenseWithCorrectDate()
    {
        DateTime specificDate = new DateTime(2024, 10, 15, 14, 30, 0, DateTimeKind.Utc);

        AddExpenseRequest request = new()
        {
            Amount = 150,
            Currency = "BGN",
            Date = specificDate,
            Description = "Entertainment",
            ExpenseType = ExpenseType.Wants
        };

        ExpenseService sut = new(CreateContext(), _logger);

        DataResult<Guid> result = await sut.AddExpenseAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        await using ApplicationDbContext assertContext = CreateContext();
        ExpenseTransaction? addedExpense = await assertContext.ExpenseTransactions.FindAsync(result.Data);
        addedExpense.Should().NotBeNull();
        addedExpense!.Date.Should().Be(specificDate);
    }
}
