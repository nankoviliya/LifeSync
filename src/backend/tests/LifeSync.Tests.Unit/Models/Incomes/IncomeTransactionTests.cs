using FluentAssertions;
using LifeSync.API.Models.Incomes;
using LifeSync.API.Shared;
using LifeSync.Common.Required;
using LifeSync.UnitTests.TestHelpers;

namespace LifeSync.UnitTests.Models.Incomes;

public class IncomeTransactionTests
{
    [Fact]
    public void From_WithValidData_ShouldCreateIncomeTransaction()
    {
        var amount = new Money(1000m, Currency.Usd).ToRequiredReference();
        var date = new DateTime(2024, 6, 15).ToRequiredStruct();
        var description = "Monthly Salary".ToRequiredString();
        var userId = "user123".ToRequiredString();

        var income = IncomeTransaction.From(amount, date, description, userId);

        income.Amount.Amount.Should().Be(1000m);
        income.Date.Should().Be(new DateTime(2024, 6, 15));
        income.Description.Should().Be("Monthly Salary");
        income.UserId.Should().Be("user123");
    }

    [Fact]
    public void From_WithNegativeAmount_ShouldThrowArgumentException()
    {
        var amount = new Money(-1000m, Currency.Usd).ToRequiredReference();

        Action act = () => IncomeTransaction.From(
            amount,
            DateTime.UtcNow.ToRequiredStruct(),
            "Test".ToRequiredString(),
            "user123".ToRequiredString());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Income amount cannot be negative.*");
    }

    [Fact]
    public void From_WithZeroAmount_ShouldThrowArgumentException()
    {
        var amount = new Money(0m, Currency.Usd).ToRequiredReference();

        Action act = () => IncomeTransaction.From(
            amount,
            DateTime.UtcNow.ToRequiredStruct(),
            "Test".ToRequiredString(),
            "user123".ToRequiredString());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Income amount must be greater than zero.*");
    }

    [Fact]
    public void From_WithFutureDate_ShouldThrowArgumentException()
    {
        var futureDate = DateTime.UtcNow.AddDays(2).ToRequiredStruct();

        Action act = () => IncomeTransaction.From(
            new Money(1000m, Currency.Usd).ToRequiredReference(),
            futureDate,
            "Test".ToRequiredString(),
            "user123".ToRequiredString());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Income date cannot be in the future.*");
    }

    [Fact]
    public void From_WithDateTooFarInPast_ShouldThrowArgumentException()
    {
        var oldDate = new DateTime(1899, 12, 31).ToRequiredStruct();

        Action act = () => IncomeTransaction.From(
            new Money(1000m, Currency.Usd).ToRequiredReference(),
            oldDate,
            "Test".ToRequiredString(),
            "user123".ToRequiredString());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Income date is too far in the past.*");
    }

    [Fact]
    public void From_WithDescriptionTooLong_ShouldThrowArgumentException()
    {
        var longDescription = new string('a', 501);

        Action act = () => IncomeTransaction.From(
            new Money(1000m, Currency.Usd).ToRequiredReference(),
            DateTime.UtcNow.ToRequiredStruct(),
            longDescription.ToRequiredString(),
            "user123".ToRequiredString());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Description cannot exceed 500 characters.*");
    }

    [Fact]
    public void UpdateAmount_WithValidAmount_ShouldUpdateAmount()
    {
        var income = CreateTestIncome();
        var newAmount = new Money(1500m, Currency.Usd).ToRequiredReference();

        income.UpdateAmount(newAmount);

        income.Amount.Amount.Should().Be(1500m);
    }

    [Fact]
    public void UpdateDate_WithValidDate_ShouldUpdateDate()
    {
        var income = CreateTestIncome();
        var newDate = new DateTime(2024, 7, 1).ToRequiredStruct();

        income.UpdateDate(newDate);

        income.Date.Should().Be(new DateTime(2024, 7, 1));
    }

    [Fact]
    public void UpdateDescription_WithValidDescription_ShouldUpdateDescription()
    {
        var income = CreateTestIncome();

        income.UpdateDescription("Updated salary".ToRequiredString());

        income.Description.Should().Be("Updated salary");
    }

    [Fact]
    public void IsWithinDateRange_WithDateInRange_ShouldReturnTrue()
    {
        var income = CreateTestIncome(date: new DateTime(2024, 6, 15));
        var startDate = new DateTime(2024, 6, 1);
        var endDate = new DateTime(2024, 6, 30);

        var result = income.IsWithinDateRange(startDate, endDate);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsWithinDateRange_WithDateOutsideRange_ShouldReturnFalse()
    {
        var income = CreateTestIncome(date: new DateTime(2024, 7, 15));
        var startDate = new DateTime(2024, 6, 1);
        var endDate = new DateTime(2024, 6, 30);

        var result = income.IsWithinDateRange(startDate, endDate);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsInMonth_WithMatchingMonth_ShouldReturnTrue()
    {
        var income = CreateTestIncome(date: new DateTime(2024, 6, 15));

        var result = income.IsInMonth(2024, 6);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsInMonth_WithDifferentMonth_ShouldReturnFalse()
    {
        var income = CreateTestIncome(date: new DateTime(2024, 6, 15));

        var result = income.IsInMonth(2024, 7);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsInYear_WithMatchingYear_ShouldReturnTrue()
    {
        var income = CreateTestIncome(date: new DateTime(2024, 6, 15));

        var result = income.IsInYear(2024);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsInYear_WithDifferentYear_ShouldReturnFalse()
    {
        var income = CreateTestIncome(date: new DateTime(2024, 6, 15));

        var result = income.IsInYear(2023);

        result.Should().BeFalse();
    }

    [Fact]
    public void CalculateNetIncome_WithValidPercentage_ShouldReturnNetAmount()
    {
        var income = CreateTestIncome(1000m);

        var netIncome = income.CalculateNetIncome(20m); // 20% deduction

        netIncome.Amount.Should().Be(800m);
    }

    [Fact]
    public void CalculateNetIncome_WithZeroPercentage_ShouldReturnFullAmount()
    {
        var income = CreateTestIncome(1000m);

        var netIncome = income.CalculateNetIncome(0m);

        netIncome.Amount.Should().Be(1000m);
    }

    [Fact]
    public void CalculateNetIncome_WithNegativePercentage_ShouldThrowArgumentException()
    {
        var income = CreateTestIncome();

        Action act = () => income.CalculateNetIncome(-10m);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Deduction percentage must be between 0 and 100.*");
    }

    [Fact]
    public void CalculateNetIncome_WithPercentageOver100_ShouldThrowArgumentException()
    {
        var income = CreateTestIncome();

        Action act = () => income.CalculateNetIncome(101m);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Deduction percentage must be between 0 and 100.*");
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var income = CreateTestIncome();

        var result = income.ToString();

        result.Should().Contain("Income");
        result.Should().Contain("Monthly Salary");
        result.Should().Contain("2024-06-15");
    }

    [Fact]
    public void Equality_WithSameId_ShouldBeEqual()
    {
        var (income1, income2) = EntityTestHelper.CreateTwoWithSameId(() =>
            IncomeTransaction.From(
                new Money(1000m, Currency.Usd).ToRequiredReference(),
                new DateTime(2024, 6, 15).ToRequiredStruct(),
                "Test".ToRequiredString(),
                "user123".ToRequiredString()));

        income1.Should().Be(income2);
        (income1 == income2).Should().BeTrue();
        income1.GetHashCode().Should().Be(income2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentId_ShouldNotBeEqual()
    {
        var (income1, income2) = EntityTestHelper.CreateTwoWithDifferentIds(() =>
            IncomeTransaction.From(
                new Money(1000m, Currency.Usd).ToRequiredReference(),
                new DateTime(2024, 6, 15).ToRequiredStruct(),
                "Test".ToRequiredString(),
                "user123".ToRequiredString()));

        income1.Should().NotBe(income2);
        (income1 == income2).Should().BeFalse();
    }

    [Fact]
    public void Equality_WithSameIdButDifferentData_ShouldBeEqual()
    {
        var income1 = IncomeTransaction.From(
            new Money(1000m, Currency.Usd).ToRequiredReference(),
            new DateTime(2024, 6, 15).ToRequiredStruct(),
            "Salary".ToRequiredString(),
            "user123".ToRequiredString());

        var income2 = IncomeTransaction.From(
            new Money(500m, Currency.Eur).ToRequiredReference(),
            new DateTime(2023, 1, 1).ToRequiredStruct(),
            "Bonus".ToRequiredString(),
            "user999".ToRequiredString());

        var sharedId = Guid.NewGuid();
        EntityTestHelper.SetId(income1, sharedId);
        EntityTestHelper.SetId(income2, sharedId);

        income1.Should().Be(income2);
    }

    private IncomeTransaction CreateTestIncome(decimal amount = 1000m, DateTime? date = null)
    {
        return IncomeTransaction.From(
            new Money(amount, Currency.Usd).ToRequiredReference(),
            (date ?? new DateTime(2024, 6, 15)).ToRequiredStruct(),
            "Monthly Salary".ToRequiredString(),
            "user123".ToRequiredString());
    }
}
