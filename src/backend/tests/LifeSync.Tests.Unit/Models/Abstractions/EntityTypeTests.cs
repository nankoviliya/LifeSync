using FluentAssertions;
using LifeSync.API.Models.Abstractions;

namespace LifeSync.Tests.Unit.Models.Abstractions;

public class EntityTypeTests
{
    [Fact]
    public void EntityType_AllValues_ShouldHaveUniqueUnderlyingInts()
    {
        Array values = Enum.GetValues(typeof(EntityType));
        int[] underlying = values.Cast<EntityType>().Select(v => (int)v).ToArray();

        underlying.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void EntityType_ExpenseTransaction_ShouldHaveExplicitValue()
    {
        ((int)EntityType.ExpenseTransaction).Should().Be(1);
    }

    [Fact]
    public void EntityType_IncomeTransaction_ShouldHaveExplicitValue()
    {
        ((int)EntityType.IncomeTransaction).Should().Be(2);
    }
}
