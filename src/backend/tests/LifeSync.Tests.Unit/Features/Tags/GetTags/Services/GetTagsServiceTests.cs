using FluentAssertions;
using LifeSync.API.Features.Tags.AssignTag.Services;
using LifeSync.API.Features.Tags.GetTags.Services;
using LifeSync.API.Features.Tags.Shared.Models;
using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Expenses;
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

namespace LifeSync.Tests.Unit.Features.Tags.GetTags.Services;

public class GetTagsServiceTests : IDisposable
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _contextOptions;
    private readonly ISecretsManager _secretsManager;
    private readonly ILogger<GetTagsService> _getLogger;
    private readonly ILogger<AssignTagService> _assignLogger;

    private readonly string _userId;
    private readonly string _otherUserId;

    public GetTagsServiceTests()
    {
        _getLogger = Substitute.For<ILogger<GetTagsService>>();
        _assignLogger = Substitute.For<ILogger<AssignTagService>>();
        _secretsManager = Substitute.For<ISecretsManager>();

        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        using ApplicationDbContext context = new(_contextOptions, _secretsManager);
        context.Database.EnsureCreated();

        Language language = Language.From("English".ToRequiredString(), "en".ToRequiredString());
        context.Add(language);

        User user = User.From(
            "user@test.com".ToRequiredString(),
            "user@test.com".ToRequiredString(),
            "Test".ToRequiredString(),
            "User".ToRequiredString(),
            new Money(1000, "BGN").ToRequiredReference(),
            "BGN".ToRequiredString(),
            language.Id.ToRequiredStruct());

        User other = User.From(
            "other@test.com".ToRequiredString(),
            "other@test.com".ToRequiredString(),
            "Other".ToRequiredString(),
            "User".ToRequiredString(),
            new Money(500, "BGN").ToRequiredReference(),
            "BGN".ToRequiredString(),
            language.Id.ToRequiredStruct());

        context.Add(user);
        context.Add(other);
        context.SaveChanges();

        _userId = user.Id;
        _otherUserId = other.Id;
    }

    private ApplicationDbContext CreateContext() => new(_contextOptions, _secretsManager);

    public void Dispose() => _connection.Dispose();

    private async Task<ExpenseTransaction> SeedExpenseAsync(string userId)
    {
        await using ApplicationDbContext context = CreateContext();
        ExpenseTransaction expense = ExpenseTransaction.From(
            new Money(10m, "USD").ToRequiredReference(),
            DateTime.UtcNow.ToRequiredStruct(),
            "test".ToRequiredString(),
            ExpenseType.Needs,
            userId.ToRequiredString());

        context.ExpenseTransactions.Add(expense);
        await context.SaveChangesAsync();
        return expense;
    }

    private async Task AssignAsync(string userId, string tagName, Guid entityId)
    {
        await using ApplicationDbContext db = CreateContext();
        AssignTagService assign = new(db, _assignLogger);
        await assign.AssignTagAsync(
            userId.ToRequiredString(),
            tagName.ToRequiredString(),
            entityId.ToRequiredStruct(),
            EntityType.ExpenseTransaction,
            CancellationToken.None);
    }

    [Fact]
    public async Task GetUserTagsAsync_WithoutPrefix_ShouldReturnAllActiveTags()
    {
        ExpenseTransaction expense = await SeedExpenseAsync(_userId);
        await AssignAsync(_userId, "dentist", expense.Id);
        await AssignAsync(_userId, "groceries", expense.Id);

        await using ApplicationDbContext db = CreateContext();
        GetTagsService sut = new(db, _getLogger);

        DataResult<IReadOnlyList<TagDto>> result = await sut.GetUserTagsAsync(
            _userId.ToRequiredString(), null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Select(t => t.Name).Should().BeEquivalentTo(new[] { "dentist", "groceries" });
    }

    [Fact]
    public async Task GetUserTagsAsync_WithPrefix_ShouldFilterByLowercasedPrefix()
    {
        ExpenseTransaction expense = await SeedExpenseAsync(_userId);
        await AssignAsync(_userId, "dentist", expense.Id);
        await AssignAsync(_userId, "groceries", expense.Id);

        await using ApplicationDbContext db = CreateContext();
        GetTagsService sut = new(db, _getLogger);

        DataResult<IReadOnlyList<TagDto>> result = await sut.GetUserTagsAsync(
            _userId.ToRequiredString(), "DEN", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().ContainSingle(t => t.Name == "dentist");
    }

    [Fact]
    public async Task GetUserTagsAsync_ShouldExcludeOtherUsersTags()
    {
        ExpenseTransaction otherExpense = await SeedExpenseAsync(_otherUserId);
        await AssignAsync(_otherUserId, "secret", otherExpense.Id);

        await using ApplicationDbContext db = CreateContext();
        GetTagsService sut = new(db, _getLogger);

        DataResult<IReadOnlyList<TagDto>> result = await sut.GetUserTagsAsync(
            _userId.ToRequiredString(), null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }
}
