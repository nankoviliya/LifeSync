using FluentAssertions;
using LifeSync.API.Models.Expenses;
using LifeSync.API.Shared;
using LifeSync.Common.Required;
using LifeSync.Tests.Unit.TestHelpers;

namespace LifeSync.Tests.Unit.Models.Expenses;

public class ExpenseTransactionTests
{
    [Fact]
    public void From_WithValidData_ShouldCreateExpenseTransaction()
    {
        var amount = new Money(50m, "USD").ToRequiredReference();
        var date = new DateTime(2024, 6, 15).ToRequiredStruct();
        var description = "Groceries".ToRequiredString();
        var expenseType = ExpenseType.Needs;
        var userId = "user123".ToRequiredString();

        var expense = ExpenseTransaction.From(amount, date, description, expenseType, userId);

        expense.Amount.Amount.Should().Be(50m);
        expense.Date.Should().Be(new DateTime(2024, 6, 15));
        expense.Description.Should().Be("Groceries");
        expense.ExpenseType.Should().Be(ExpenseType.Needs);
        expense.UserId.Should().Be("user123");
    }

    [Fact]
    public void From_WithNegativeAmount_ShouldThrowArgumentException()
    {
        var amount = new Money(-50m, "USD").ToRequiredReference();

        Action act = () => ExpenseTransaction.From(
            amount,
            DateTime.UtcNow.ToRequiredStruct(),
            "Test".ToRequiredString(),
            ExpenseType.Needs,
            "user123".ToRequiredString());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Expense amount cannot be negative.*");
    }

    [Fact]
    public void From_WithZeroAmount_ShouldThrowArgumentException()
    {
        var amount = new Money(0m, "USD").ToRequiredReference();

        Action act = () => ExpenseTransaction.From(
            amount,
            DateTime.UtcNow.ToRequiredStruct(),
            "Test".ToRequiredString(),
            ExpenseType.Needs,
            "user123".ToRequiredString());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Expense amount must be greater than zero.*");
    }

    [Fact]
    public void From_WithFutureDate_ShouldThrowArgumentException()
    {
        var futureDate = DateTime.UtcNow.AddDays(2).ToRequiredStruct();

        Action act = () => ExpenseTransaction.From(
            new Money(50m, "USD").ToRequiredReference(),
            futureDate,
            "Test".ToRequiredString(),
            ExpenseType.Needs,
            "user123".ToRequiredString());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Expense date cannot be in the future.*");
    }

    [Fact]
    public void From_WithDateTooFarInPast_ShouldThrowArgumentException()
    {
        var oldDate = new DateTime(1899, 12, 31).ToRequiredStruct();

        Action act = () => ExpenseTransaction.From(
            new Money(50m, "USD").ToRequiredReference(),
            oldDate,
            "Test".ToRequiredString(),
            ExpenseType.Needs,
            "user123".ToRequiredString());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Expense date is too far in the past.*");
    }

    [Fact]
    public void From_WithDescriptionTooLong_ShouldThrowArgumentException()
    {
        var longDescription = new string('a', 501);

        Action act = () => ExpenseTransaction.From(
            new Money(50m, "USD").ToRequiredReference(),
            DateTime.UtcNow.ToRequiredStruct(),
            longDescription.ToRequiredString(),
            ExpenseType.Needs,
            "user123".ToRequiredString());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Description cannot exceed 500 characters.*");
    }

    [Fact]
    public void UpdateAmount_WithValidAmount_ShouldUpdateAmount()
    {
        var expense = CreateTestExpense();
        var newAmount = new Money(75m, "USD").ToRequiredReference();

        expense.UpdateAmount(newAmount);

        expense.Amount.Amount.Should().Be(75m);
    }

    [Fact]
    public void UpdateDate_WithValidDate_ShouldUpdateDate()
    {
        var expense = CreateTestExpense();
        var newDate = new DateTime(2024, 7, 1).ToRequiredStruct();

        expense.UpdateDate(newDate);

        expense.Date.Should().Be(new DateTime(2024, 7, 1));
    }

    [Fact]
    public void UpdateDescription_WithValidDescription_ShouldUpdateDescription()
    {
        var expense = CreateTestExpense();

        expense.UpdateDescription("Updated description".ToRequiredString());

        expense.Description.Should().Be("Updated description");
    }

    [Fact]
    public void ChangeExpenseType_WithValidType_ShouldUpdateType()
    {
        var expense = CreateTestExpense();

        expense.ChangeExpenseType(ExpenseType.Wants);

        expense.ExpenseType.Should().Be(ExpenseType.Wants);
    }

    [Fact]
    public void IsNeed_WithNeedsExpenseType_ShouldReturnTrue()
    {
        var expense = CreateTestExpense(ExpenseType.Needs);

        expense.IsNeed().Should().BeTrue();
        expense.IsWant().Should().BeFalse();
        expense.IsSavings().Should().BeFalse();
    }

    [Fact]
    public void IsWant_WithWantsExpenseType_ShouldReturnTrue()
    {
        var expense = CreateTestExpense(ExpenseType.Wants);

        expense.IsWant().Should().BeTrue();
        expense.IsNeed().Should().BeFalse();
        expense.IsSavings().Should().BeFalse();
    }

    [Fact]
    public void IsSavings_WithSavingsExpenseType_ShouldReturnTrue()
    {
        var expense = CreateTestExpense(ExpenseType.Savings);

        expense.IsSavings().Should().BeTrue();
        expense.IsNeed().Should().BeFalse();
        expense.IsWant().Should().BeFalse();
    }

    [Fact]
    public void IsWithinDateRange_WithDateInRange_ShouldReturnTrue()
    {
        var expense = CreateTestExpense(date: new DateTime(2024, 6, 15));
        var startDate = new DateTime(2024, 6, 1);
        var endDate = new DateTime(2024, 6, 30);

        var result = expense.IsWithinDateRange(startDate, endDate);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsWithinDateRange_WithDateOutsideRange_ShouldReturnFalse()
    {
        var expense = CreateTestExpense(date: new DateTime(2024, 7, 15));
        var startDate = new DateTime(2024, 6, 1);
        var endDate = new DateTime(2024, 6, 30);

        var result = expense.IsWithinDateRange(startDate, endDate);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsInMonth_WithMatchingMonth_ShouldReturnTrue()
    {
        var expense = CreateTestExpense(date: new DateTime(2024, 6, 15));

        var result = expense.IsInMonth(2024, 6);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsInMonth_WithDifferentMonth_ShouldReturnFalse()
    {
        var expense = CreateTestExpense(date: new DateTime(2024, 6, 15));

        var result = expense.IsInMonth(2024, 7);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsInYear_WithMatchingYear_ShouldReturnTrue()
    {
        var expense = CreateTestExpense(date: new DateTime(2024, 6, 15));

        var result = expense.IsInYear(2024);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsInYear_WithDifferentYear_ShouldReturnFalse()
    {
        var expense = CreateTestExpense(date: new DateTime(2024, 6, 15));

        var result = expense.IsInYear(2023);

        result.Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var expense = CreateTestExpense();

        var result = expense.ToString();

        result.Should().Contain("Needs");
        result.Should().Contain("Groceries");
        result.Should().Contain("2024-06-15");
    }

    [Fact]
    public void Equality_WithSameId_ShouldBeEqual()
    {
        var (expense1, expense2) = EntityTestHelper.CreateTwoWithSameId(() =>
            ExpenseTransaction.From(
                new Money(50m, "USD").ToRequiredReference(),
                new DateTime(2024, 6, 15).ToRequiredStruct(),
                "Test".ToRequiredString(),
                ExpenseType.Needs,
                "user123".ToRequiredString()));

        expense1.Should().Be(expense2);
        (expense1 == expense2).Should().BeTrue();
        expense1.GetHashCode().Should().Be(expense2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentId_ShouldNotBeEqual()
    {
        var (expense1, expense2) = EntityTestHelper.CreateTwoWithDifferentIds(() =>
            ExpenseTransaction.From(
                new Money(50m, "USD").ToRequiredReference(),
                new DateTime(2024, 6, 15).ToRequiredStruct(),
                "Test".ToRequiredString(),
                ExpenseType.Needs,
                "user123".ToRequiredString()));

        expense1.Should().NotBe(expense2);
        (expense1 == expense2).Should().BeFalse();
    }

    [Fact]
    public void Equality_WithSameIdButDifferentData_ShouldBeEqual()
    {
        var expense1 = ExpenseTransaction.From(
            new Money(50m, "USD").ToRequiredReference(),
            new DateTime(2024, 6, 15).ToRequiredStruct(),
            "Groceries".ToRequiredString(),
            ExpenseType.Needs,
            "user123".ToRequiredString());

        var expense2 = ExpenseTransaction.From(
            new Money(999m, "EUR").ToRequiredReference(),
            new DateTime(2023, 1, 1).ToRequiredStruct(),
            "Entertainment".ToRequiredString(),
            ExpenseType.Wants,
            "user999".ToRequiredString());

        var sharedId = Guid.NewGuid();
        EntityTestHelper.SetId(expense1, sharedId);
        EntityTestHelper.SetId(expense2, sharedId);

        expense1.Should().Be(expense2);
    }

    private ExpenseTransaction CreateTestExpense(ExpenseType type = ExpenseType.Needs, DateTime? date = null)
    {
        return ExpenseTransaction.From(
            new Money(50m, "USD").ToRequiredReference(),
            (date ?? new DateTime(2024, 6, 15)).ToRequiredStruct(),
            "Groceries".ToRequiredString(),
            type,
            "user123".ToRequiredString());
    }
}
