using FluentAssertions;
using LifeSync.API.Features.Tags.AssignTag.Services;
using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Expenses;
using LifeSync.API.Models.Languages;
using LifeSync.API.Models.Tags;
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

namespace LifeSync.Tests.Unit.Features.Tags.AssignTag.Services;

public class AssignTagServiceTests : IDisposable
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _contextOptions;
    private readonly ISecretsManager _secretsManager;
    private readonly ILogger<AssignTagService> _logger;

    private readonly string _ownerUserId;
    private readonly string _intruderUserId;

    public AssignTagServiceTests()
    {
        _logger = Substitute.For<ILogger<AssignTagService>>();
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

        User owner = User.From(
            "owner@test.com".ToRequiredString(),
            "owner@test.com".ToRequiredString(),
            "Owner".ToRequiredString(),
            "User".ToRequiredString(),
            new Money(1000, "BGN").ToRequiredReference(),
            "BGN".ToRequiredString(),
            language.Id.ToRequiredStruct());

        User intruder = User.From(
            "intruder@test.com".ToRequiredString(),
            "intruder@test.com".ToRequiredString(),
            "Intruder".ToRequiredString(),
            "User".ToRequiredString(),
            new Money(500, "BGN").ToRequiredReference(),
            "BGN".ToRequiredString(),
            language.Id.ToRequiredStruct());

        context.Add(owner);
        context.Add(intruder);
        context.SaveChanges();

        _ownerUserId = owner.Id;
        _intruderUserId = intruder.Id;
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

    [Fact]
    public async Task AssignTagAsync_WhenTagDoesNotExist_ShouldCreateTagAndAssignment()
    {
        ExpenseTransaction expense = await SeedExpenseAsync(_ownerUserId);
        await using ApplicationDbContext db = CreateContext();
        AssignTagService sut = new(db, _logger);

        DataResult<Guid> result = await sut.AssignTagAsync(
            _ownerUserId.ToRequiredString(),
            "dentist".ToRequiredString(),
            expense.Id.ToRequiredStruct(),
            EntityType.ExpenseTransaction,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        await using ApplicationDbContext verify = CreateContext();
        verify.Tags.Single().Name.Should().Be("dentist");
        verify.TagAssignments.Single().TagId.Should().Be(verify.Tags.Single().Id);
    }

    [Fact]
    public async Task AssignTagAsync_WhenTagAlreadyExistsActive_ShouldReuseTag()
    {
        ExpenseTransaction expense = await SeedExpenseAsync(_ownerUserId);

        await using (ApplicationDbContext seedDb = CreateContext())
        {
            Tag existing = Tag.From("dentist".ToRequiredString(), _ownerUserId.ToRequiredString());
            seedDb.Tags.Add(existing);
            await seedDb.SaveChangesAsync();
        }

        await using ApplicationDbContext db = CreateContext();
        AssignTagService sut = new(db, _logger);

        await sut.AssignTagAsync(
            _ownerUserId.ToRequiredString(),
            "dentist".ToRequiredString(),
            expense.Id.ToRequiredStruct(),
            EntityType.ExpenseTransaction,
            CancellationToken.None);

        await using ApplicationDbContext verify = CreateContext();
        verify.Tags.Should().HaveCount(1);
        Guid expectedTagId = verify.Tags.Single().Id;
        verify.TagAssignments.Single().TagId.Should().Be(expectedTagId);
    }

    [Fact]
    public async Task AssignTagAsync_WhenTagSoftDeleted_ShouldRestoreAndReuse()
    {
        ExpenseTransaction expense = await SeedExpenseAsync(_ownerUserId);

        await using (ApplicationDbContext seedDb = CreateContext())
        {
            Tag existing = Tag.From("dentist".ToRequiredString(), _ownerUserId.ToRequiredString());
            existing.MarkAsDeleted();
            seedDb.Tags.Add(existing);
            await seedDb.SaveChangesAsync();
        }

        await using ApplicationDbContext db = CreateContext();
        AssignTagService sut = new(db, _logger);

        await sut.AssignTagAsync(
            _ownerUserId.ToRequiredString(),
            "dentist".ToRequiredString(),
            expense.Id.ToRequiredStruct(),
            EntityType.ExpenseTransaction,
            CancellationToken.None);

        await using ApplicationDbContext verify = CreateContext();
        Tag reloaded = verify.Tags.IgnoreQueryFilters().Single();
        reloaded.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task AssignTagAsync_WhenAssignmentAlreadyExists_ShouldBeIdempotent()
    {
        ExpenseTransaction expense = await SeedExpenseAsync(_ownerUserId);

        DataResult<Guid> first;
        DataResult<Guid> second;

        await using (ApplicationDbContext db1 = CreateContext())
        {
            AssignTagService sut1 = new(db1, _logger);
            first = await sut1.AssignTagAsync(
                _ownerUserId.ToRequiredString(),
                "dentist".ToRequiredString(),
                expense.Id.ToRequiredStruct(),
                EntityType.ExpenseTransaction,
                CancellationToken.None);
        }

        await using (ApplicationDbContext db2 = CreateContext())
        {
            AssignTagService sut2 = new(db2, _logger);
            second = await sut2.AssignTagAsync(
                _ownerUserId.ToRequiredString(),
                "dentist".ToRequiredString(),
                expense.Id.ToRequiredStruct(),
                EntityType.ExpenseTransaction,
                CancellationToken.None);
        }

        second.IsSuccess.Should().BeTrue();
        second.Data.Should().Be(first.Data);

        await using ApplicationDbContext verify = CreateContext();
        verify.TagAssignments.Should().HaveCount(1);
    }

    [Fact]
    public async Task AssignTagAsync_WhenEntityNotOwnedByUser_ShouldFail()
    {
        ExpenseTransaction expense = await SeedExpenseAsync(_ownerUserId);
        await using ApplicationDbContext db = CreateContext();
        AssignTagService sut = new(db, _logger);

        DataResult<Guid> result = await sut.AssignTagAsync(
            _intruderUserId.ToRequiredString(),
            "dentist".ToRequiredString(),
            expense.Id.ToRequiredStruct(),
            EntityType.ExpenseTransaction,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();

        await using ApplicationDbContext verify = CreateContext();
        verify.Tags.Should().BeEmpty();
        verify.TagAssignments.Should().BeEmpty();
    }

    [Fact]
    public async Task AssignTagAsync_WhenInvalidTagName_ShouldFail()
    {
        ExpenseTransaction expense = await SeedExpenseAsync(_ownerUserId);
        await using ApplicationDbContext db = CreateContext();
        AssignTagService sut = new(db, _logger);

        DataResult<Guid> result = await sut.AssignTagAsync(
            _ownerUserId.ToRequiredString(),
            "Bad Name!".ToRequiredString(),
            expense.Id.ToRequiredStruct(),
            EntityType.ExpenseTransaction,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}
