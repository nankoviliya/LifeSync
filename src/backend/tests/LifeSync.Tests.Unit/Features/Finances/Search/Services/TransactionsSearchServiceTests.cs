using FluentAssertions;
using LifeSync.API.Features.Finances.Search.Models;
using LifeSync.API.Features.Finances.Search.Services;
using LifeSync.API.Features.Finances.Shared.Models;
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

namespace LifeSync.UnitTests.Features.Finances.Search.Services;

public class TransactionsSearchServiceTests
{
    private readonly Guid _testLanguageId;
    private readonly string _testUserId = default!;

    private readonly DbConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _contextOptions;

    private readonly ILogger<TransactionsSearchService> _logger;
    private readonly ISecretsManager _secretsManager;

    public TransactionsSearchServiceTests()
    {
        _logger = Substitute.For<ILogger<TransactionsSearchService>>();
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
                "Test".ToRequiredString(),
                "User".ToRequiredString(),
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

    private async Task SeedTestDataAsync()
    {
        await using ApplicationDbContext context = CreateContext();

        ExpenseTransaction expense1 = ExpenseTransaction.From(
            new Money(100, "BGN").ToRequiredReference(),
            new DateTime(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc).ToRequiredStruct(),
            "Groceries".ToRequiredString(),
            ExpenseType.Needs,
            _testUserId.ToRequiredString()
        );

        ExpenseTransaction expense2 = ExpenseTransaction.From(
            new Money(200, "BGN").ToRequiredReference(),
            new DateTime(2024, 10, 5, 0, 0, 0, DateTimeKind.Utc).ToRequiredStruct(),
            "Entertainment".ToRequiredString(),
            ExpenseType.Wants,
            _testUserId.ToRequiredString()
        );

        ExpenseTransaction expense3 = ExpenseTransaction.From(
            new Money(300, "BGN").ToRequiredReference(),
            new DateTime(2024, 10, 10, 0, 0, 0, DateTimeKind.Utc).ToRequiredStruct(),
            "Investment".ToRequiredString(),
            ExpenseType.Savings,
            _testUserId.ToRequiredString()
        );

        IncomeTransaction income1 = IncomeTransaction.From(
            new Money(1500, "BGN").ToRequiredReference(),
            new DateTime(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc).ToRequiredStruct(),
            "Salary".ToRequiredString(),
            _testUserId.ToRequiredString()
        );

        IncomeTransaction income2 = IncomeTransaction.From(
            new Money(500, "BGN").ToRequiredReference(),
            new DateTime(2024, 10, 15, 0, 0, 0, DateTimeKind.Utc).ToRequiredStruct(),
            "Bonus".ToRequiredString(),
            _testUserId.ToRequiredString()
        );

        context.ExpenseTransactions.AddRange(expense1, expense2, expense3);
        context.IncomeTransactions.AddRange(income1, income2);

        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task SearchTransactionsAsync_ShouldReturnAllExpenses_WhenFilteringByExpenseType()
    {
        await SeedTestDataAsync();

        SearchTransactionsRequest request = new()
        {
            Filters = new SearchTransactionsFilters
            {
                TransactionTypes = [TransactionType.Expense]
            }
        };

        TransactionsSearchService sut = new(CreateContext(), _logger);

        DataResult<SearchTransactionsResponse> result =
            await sut.SearchTransactionsAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Transactions.Should().HaveCount(3);
        result.Data.Transactions.Should().AllBeOfType<ExpenseTransactionDto>();
        result.Data.TransactionsCount.Should().Be(3);
    }

    [Fact]
    public async Task SearchTransactionsAsync_ShouldReturnAllIncomes_WhenFilteringByIncomeType()
    {
        await SeedTestDataAsync();

        SearchTransactionsRequest request = new()
        {
            Filters = new SearchTransactionsFilters
            {
                TransactionTypes = [TransactionType.Income]
            }
        };

        TransactionsSearchService sut = new(CreateContext(), _logger);

        DataResult<SearchTransactionsResponse> result =
            await sut.SearchTransactionsAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.Transactions.Should().HaveCount(2);
        result.Data.Transactions.Should().AllBeOfType<IncomeTransactionDto>();
        result.Data.TransactionsCount.Should().Be(2);
    }

    [Fact]
    public async Task SearchTransactionsAsync_ShouldReturnBothExpensesAndIncomes_WhenBothTypesSpecified()
    {
        await SeedTestDataAsync();

        SearchTransactionsRequest request = new()
        {
            Filters = new SearchTransactionsFilters
            {
                TransactionTypes = [TransactionType.Expense, TransactionType.Income]
            }
        };

        TransactionsSearchService sut = new(CreateContext(), _logger);

        DataResult<SearchTransactionsResponse> result =
            await sut.SearchTransactionsAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.Transactions.Should().HaveCount(5);
        result.Data.TransactionsCount.Should().Be(5);
    }

    [Fact]
    public async Task SearchTransactionsAsync_ShouldCalculateExpenseSummaryCorrectly()
    {
        await SeedTestDataAsync();

        SearchTransactionsRequest request = new()
        {
            Filters = new SearchTransactionsFilters
            {
                TransactionTypes = [TransactionType.Expense]
            }
        };

        TransactionsSearchService sut = new(CreateContext(), _logger);

        DataResult<SearchTransactionsResponse> result =
            await sut.SearchTransactionsAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.ExpenseSummary.Should().NotBeNull();
        result.Data.ExpenseSummary.TotalSpent.Should().Be(600); // 100 + 200 + 300
        result.Data.ExpenseSummary.TotalSpentOnNeeds.Should().Be(100);
        result.Data.ExpenseSummary.TotalSpentOnWants.Should().Be(200);
        result.Data.ExpenseSummary.TotalSpentOnSavings.Should().Be(300);
        result.Data.ExpenseSummary.Currency.Should().Be("BGN");
    }

    [Fact]
    public async Task SearchTransactionsAsync_ShouldCalculateIncomeSummaryCorrectly()
    {
        await SeedTestDataAsync();

        SearchTransactionsRequest request = new()
        {
            Filters = new SearchTransactionsFilters
            {
                TransactionTypes = [TransactionType.Income]
            }
        };

        TransactionsSearchService sut = new(CreateContext(), _logger);

        DataResult<SearchTransactionsResponse> result =
            await sut.SearchTransactionsAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.IncomeSummary.Should().NotBeNull();
        result.Data.IncomeSummary.TotalIncome.Should().Be(2000); // 1500 + 500
        result.Data.IncomeSummary.Currency.Should().Be("BGN");
    }

    [Fact]
    public async Task SearchTransactionsAsync_ShouldFilterByDescription()
    {
        await SeedTestDataAsync();

        SearchTransactionsRequest request = new()
        {
            Filters = new SearchTransactionsFilters
            {
                TransactionTypes = [TransactionType.Expense],
                Description = "Groceries"
            }
        };

        TransactionsSearchService sut = new(CreateContext(), _logger);

        DataResult<SearchTransactionsResponse> result =
            await sut.SearchTransactionsAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Transactions.Should().HaveCount(1);
        result.Data.Transactions[0].Description.Should().Be("Groceries");
    }

    [Fact]
    public async Task SearchTransactionsAsync_ShouldFilterByStartDate()
    {
        await SeedTestDataAsync();

        SearchTransactionsRequest request = new()
        {
            Filters = new SearchTransactionsFilters
            {
                TransactionTypes = [TransactionType.Expense],
                StartDate = new DateTime(2024, 10, 5, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        TransactionsSearchService sut = new(CreateContext(), _logger);

        DataResult<SearchTransactionsResponse> result =
            await sut.SearchTransactionsAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Transactions.Should().HaveCount(2); // Entertainment and Investment
    }

    [Fact]
    public async Task SearchTransactionsAsync_ShouldFilterByEndDate()
    {
        await SeedTestDataAsync();

        SearchTransactionsRequest request = new()
        {
            Filters = new SearchTransactionsFilters
            {
                TransactionTypes = [TransactionType.Expense],
                EndDate = new DateTime(2024, 10, 5, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        TransactionsSearchService sut = new(CreateContext(), _logger);

        DataResult<SearchTransactionsResponse> result =
            await sut.SearchTransactionsAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Transactions.Should().HaveCount(2); // Groceries and Entertainment
    }

    [Fact]
    public async Task SearchTransactionsAsync_ShouldFilterByDateRange()
    {
        await SeedTestDataAsync();

        SearchTransactionsRequest request = new()
        {
            Filters = new SearchTransactionsFilters
            {
                TransactionTypes = [TransactionType.Expense],
                StartDate = new DateTime(2024, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2024, 10, 9, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        TransactionsSearchService sut = new(CreateContext(), _logger);

        DataResult<SearchTransactionsResponse> result =
            await sut.SearchTransactionsAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Transactions.Should().HaveCount(1); // Only Entertainment
        result.Data.Transactions[0].Description.Should().Be("Entertainment");
    }

    [Fact]
    public async Task SearchTransactionsAsync_ShouldFilterByExpenseTypes()
    {
        await SeedTestDataAsync();

        SearchTransactionsRequest request = new()
        {
            Filters = new SearchTransactionsFilters
            {
                TransactionTypes = [TransactionType.Expense],
                ExpenseTypes = [ExpenseType.Needs, ExpenseType.Wants]
            }
        };

        TransactionsSearchService sut = new(CreateContext(), _logger);

        DataResult<SearchTransactionsResponse> result =
            await sut.SearchTransactionsAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Transactions.Should().HaveCount(2); // Groceries and Entertainment
        result.Data.Transactions.Should().NotContain(t => t.Description == "Investment");
    }

    [Fact]
    public async Task SearchTransactionsAsync_ShouldReturnTransactionsOrderedByDateDescending()
    {
        await SeedTestDataAsync();

        SearchTransactionsRequest request = new()
        {
            Filters = new SearchTransactionsFilters
            {
                TransactionTypes = [TransactionType.Expense, TransactionType.Income]
            }
        };

        TransactionsSearchService sut = new(CreateContext(), _logger);

        DataResult<SearchTransactionsResponse> result =
            await sut.SearchTransactionsAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Transactions.Should().HaveCount(5);

        List<DateTime> dates = result.Data.Transactions
            .Select(t => DateTime.Parse(t.Date))
            .ToList();

        dates.Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task SearchTransactionsAsync_ShouldReturnEmptyList_WhenNoTransactionsMatchFilters()
    {
        await SeedTestDataAsync();

        SearchTransactionsRequest request = new()
        {
            Filters = new SearchTransactionsFilters
            {
                TransactionTypes = [TransactionType.Expense],
                Description = "NonExistentDescription"
            }
        };

        TransactionsSearchService sut = new(CreateContext(), _logger);

        DataResult<SearchTransactionsResponse> result =
            await sut.SearchTransactionsAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Transactions.Should().BeEmpty();
        result.Data.TransactionsCount.Should().Be(0);
    }

    [Fact]
    public async Task SearchTransactionsAsync_ShouldReturnEmptyResponse_WhenNoTransactionTypesSpecified()
    {
        await SeedTestDataAsync();

        SearchTransactionsRequest request = new()
        {
            Filters = new SearchTransactionsFilters
            {
                TransactionTypes = []
            }
        };

        TransactionsSearchService sut = new(CreateContext(), _logger);

        DataResult<SearchTransactionsResponse> result =
            await sut.SearchTransactionsAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Transactions.Should().BeEmpty();
        result.Data.TransactionsCount.Should().Be(0);
    }

    [Fact]
    public async Task SearchTransactionsAsync_ShouldNotTrackEntities()
    {
        await SeedTestDataAsync();

        SearchTransactionsRequest request = new()
        {
            Filters = new SearchTransactionsFilters
            {
                TransactionTypes = [TransactionType.Expense, TransactionType.Income]
            }
        };

        ApplicationDbContext context = CreateContext();
        TransactionsSearchService sut = new(context, _logger);

        await sut.SearchTransactionsAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        context.ChangeTracker.Entries().Should().BeEmpty();
    }

    [Fact]
    public async Task SearchTransactionsAsync_ShouldOnlyReturnTransactionsForSpecifiedUser()
    {
        await SeedTestDataAsync();

        // Add another user with transactions
        await using ApplicationDbContext seedContext = CreateContext();

        User anotherUser = User.From(
            "another@test.com".ToRequiredString(),
            "another@test.com".ToRequiredString(),
            "Another".ToRequiredString(),
            "User".ToRequiredString(),
            new Money(500, "BGN").ToRequiredReference(),
            "BGN".ToRequiredString(),
            _testLanguageId.ToRequiredStruct()
        );

        seedContext.Users.Add(anotherUser);
        await seedContext.SaveChangesAsync();

        ExpenseTransaction anotherExpense = ExpenseTransaction.From(
            new Money(50, "BGN").ToRequiredReference(),
            DateTime.UtcNow.ToRequiredStruct(),
            "Another user expense".ToRequiredString(),
            ExpenseType.Needs,
            anotherUser.Id.ToRequiredString()
        );

        seedContext.ExpenseTransactions.Add(anotherExpense);
        await seedContext.SaveChangesAsync();

        SearchTransactionsRequest request = new()
        {
            Filters = new SearchTransactionsFilters
            {
                TransactionTypes = [TransactionType.Expense]
            }
        };

        TransactionsSearchService sut = new(CreateContext(), _logger);

        DataResult<SearchTransactionsResponse> result =
            await sut.SearchTransactionsAsync(_testUserId.ToRequiredString(), request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Transactions.Should().HaveCount(3); // Only the test user's expenses
        result.Data.Transactions.Should().NotContain(t => t.Description == "Another user expense");
    }
}
