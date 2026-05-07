using FluentAssertions;
using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.Expenses;
using LifeSync.API.Shared;
using LifeSync.Common.Required;
using LifeSync.Tests.Unit.TestHelpers;

namespace LifeSync.Tests.Unit.Models.Abstractions;

public class EntityRefTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateRef()
    {
        Guid entityId = Guid.NewGuid();

        EntityRef entityRef = new(entityId, EntityType.ExpenseTransaction);

        entityRef.EntityId.Should().Be(entityId);
        entityRef.EntityType.Should().Be(EntityType.ExpenseTransaction);
    }

    [Fact]
    public void Constructor_WithEmptyEntityId_ShouldThrow()
    {
        Action act = () => _ = new EntityRef(Guid.Empty, EntityType.ExpenseTransaction);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithUndefinedEntityType_ShouldThrow()
    {
        Action act = () => _ = new EntityRef(Guid.NewGuid(), (EntityType)999);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithDomainEntity_ShouldReadIdAndType()
    {
        ExpenseTransaction expense = ExpenseTransaction.From(
            new Money(50m, "USD").ToRequiredReference(),
            new DateTime(2024, 6, 15).ToRequiredStruct(),
            "Groceries".ToRequiredString(),
            ExpenseType.Needs,
            "user-1".ToRequiredString());
        EntityTestHelper.SetUniqueId(expense);

        EntityRef entityRef = EntityRef.From(expense);

        entityRef.EntityId.Should().Be(expense.Id);
        entityRef.EntityType.Should().Be(EntityType.ExpenseTransaction);
    }

    [Fact]
    public void Equality_SamePair_ShouldBeEqual()
    {
        Guid id = Guid.NewGuid();

        EntityRef a = new(id, EntityType.IncomeTransaction);
        EntityRef b = new(id, EntityType.IncomeTransaction);

        a.Should().Be(b);
        (a == b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentEntityType_ShouldNotBeEqual()
    {
        Guid id = Guid.NewGuid();

        EntityRef expense = new(id, EntityType.ExpenseTransaction);
        EntityRef income = new(id, EntityType.IncomeTransaction);

        expense.Should().NotBe(income);
    }
}
