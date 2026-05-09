using FluentAssertions;
using LifeSync.API.Features.Tags.AssignTag.Services;
using LifeSync.API.Features.Tags.UnassignTag.Services;
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

namespace LifeSync.Tests.Unit.Features.Tags.UnassignTag.Services;

public class UnassignTagServiceTests : IDisposable
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _contextOptions;
    private readonly ISecretsManager _secretsManager;
    private readonly ILogger<UnassignTagService> _unassignLogger;
    private readonly ILogger<AssignTagService> _assignLogger;

    private readonly string _ownerUserId;
    private readonly string _intruderUserId;

    public UnassignTagServiceTests()
    {
        _unassignLogger = Substitute.For<ILogger<UnassignTagService>>();
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

    private async Task<Guid> AssignAsync(string userId, string tagName, Guid entityId)
    {
        await using ApplicationDbContext db = CreateContext();
        AssignTagService assign = new(db, _assignLogger);
        DataResult<Guid> result = await assign.AssignTagAsync(
            userId.ToRequiredString(),
            tagName.ToRequiredString(),
            entityId.ToRequiredStruct(),
            EntityType.ExpenseTransaction,
            CancellationToken.None);
        return result.Data;
    }

    [Fact]
    public async Task UnassignTagAsync_WhenAssignmentExists_ShouldRemoveIt()
    {
        ExpenseTransaction expense = await SeedExpenseAsync(_ownerUserId);
        Guid assignmentId = await AssignAsync(_ownerUserId, "dentist", expense.Id);

        await using ApplicationDbContext db = CreateContext();
        UnassignTagService sut = new(db, _unassignLogger);

        DataResult<bool> result = await sut.UnassignTagAsync(
            _ownerUserId.ToRequiredString(),
            assignmentId.ToRequiredStruct(),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        await using ApplicationDbContext verify = CreateContext();
        verify.TagAssignments.Should().BeEmpty();
    }

    [Fact]
    public async Task UnassignTagAsync_WhenLastAssignmentForTag_ShouldSoftDeleteTag()
    {
        ExpenseTransaction expense = await SeedExpenseAsync(_ownerUserId);
        Guid assignmentId = await AssignAsync(_ownerUserId, "dentist", expense.Id);

        await using ApplicationDbContext db = CreateContext();
        UnassignTagService sut = new(db, _unassignLogger);

        await sut.UnassignTagAsync(
            _ownerUserId.ToRequiredString(),
            assignmentId.ToRequiredStruct(),
            CancellationToken.None);

        await using ApplicationDbContext verify = CreateContext();
        Tag tag = verify.Tags.IgnoreQueryFilters().Single();
        tag.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task UnassignTagAsync_WhenOtherAssignmentsRemain_ShouldNotSoftDeleteTag()
    {
        ExpenseTransaction first = await SeedExpenseAsync(_ownerUserId);
        ExpenseTransaction second = await SeedExpenseAsync(_ownerUserId);

        Guid firstAssignmentId = await AssignAsync(_ownerUserId, "dentist", first.Id);
        await AssignAsync(_ownerUserId, "dentist", second.Id);

        await using ApplicationDbContext db = CreateContext();
        UnassignTagService sut = new(db, _unassignLogger);

        await sut.UnassignTagAsync(
            _ownerUserId.ToRequiredString(),
            firstAssignmentId.ToRequiredStruct(),
            CancellationToken.None);

        await using ApplicationDbContext verify = CreateContext();
        verify.Tags.Single().IsDeleted.Should().BeFalse();
        verify.TagAssignments.Should().HaveCount(1);
    }

    [Fact]
    public async Task UnassignTagAsync_WhenAssignmentDoesNotExist_ShouldFail()
    {
        await using ApplicationDbContext db = CreateContext();
        UnassignTagService sut = new(db, _unassignLogger);

        DataResult<bool> result = await sut.UnassignTagAsync(
            _ownerUserId.ToRequiredString(),
            Guid.NewGuid().ToRequiredStruct(),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task UnassignTagAsync_WhenAssignmentBelongsToOtherUser_ShouldFail()
    {
        ExpenseTransaction expense = await SeedExpenseAsync(_ownerUserId);
        Guid assignmentId = await AssignAsync(_ownerUserId, "dentist", expense.Id);

        await using ApplicationDbContext db = CreateContext();
        UnassignTagService sut = new(db, _unassignLogger);

        DataResult<bool> result = await sut.UnassignTagAsync(
            _intruderUserId.ToRequiredString(),
            assignmentId.ToRequiredStruct(),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();

        await using ApplicationDbContext verify = CreateContext();
        verify.TagAssignments.Should().HaveCount(1);
    }
}
