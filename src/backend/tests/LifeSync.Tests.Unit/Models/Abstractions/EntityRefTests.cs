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
    public void From_WithDomainEntity_ShouldReadIdAndType()
    {
        ExpenseTransaction expense = BuildExpense();
        EntityTestHelper.SetUniqueId(expense);

        EntityRef entityRef = EntityRef.From(expense);

        entityRef.EntityId.Should().Be(expense.Id);
        entityRef.EntityType.Should().Be(EntityType.ExpenseTransaction);
    }

    [Fact]
    public void From_WithNullEntity_ShouldThrow()
    {
        Action act = () => _ = EntityRef.From(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void From_WithEmptyEntityId_ShouldThrow()
    {
        ExpenseTransaction expense = BuildExpense();
        // Id defaults to Guid.Empty when SetUniqueId is not called.

        Action act = () => _ = EntityRef.From(expense);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Equality_SamePair_ShouldBeEqual()
    {
        ExpenseTransaction expense = BuildExpense();
        EntityTestHelper.SetUniqueId(expense);

        EntityRef firstRef = EntityRef.From(expense);
        EntityRef secondRef = EntityRef.From(expense);

        firstRef.Should().Be(secondRef);
        (firstRef == secondRef).Should().BeTrue();
        firstRef.GetHashCode().Should().Be(secondRef.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentEntities_ShouldNotBeEqual()
    {
        ExpenseTransaction firstExpense = BuildExpense();
        ExpenseTransaction secondExpense = BuildExpense();
        EntityTestHelper.SetUniqueId(firstExpense);
        EntityTestHelper.SetUniqueId(secondExpense);

        EntityRef firstRef = EntityRef.From(firstExpense);
        EntityRef secondRef = EntityRef.From(secondExpense);

        firstRef.Should().NotBe(secondRef);
    }

    private static ExpenseTransaction BuildExpense() =>
        ExpenseTransaction.From(
            new Money(50m, "USD").ToRequiredReference(),
            new DateTime(2024, 6, 15).ToRequiredStruct(),
            "Groceries".ToRequiredString(),
            ExpenseType.Needs,
            "user-1".ToRequiredString());
}
