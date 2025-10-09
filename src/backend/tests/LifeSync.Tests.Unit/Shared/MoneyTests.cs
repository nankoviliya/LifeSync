using FluentAssertions;
using LifeSync.API.Shared;

namespace LifeSync.UnitTests.Shared;

public class MoneyTests
{
    [Fact]
    public void Constructor_WithValidAmountAndCurrency_ShouldCreateMoney()
    {
        var money = new Money(100.50m, "USD");

        money.Amount.Should().Be(100.50m);
        money.CurrencyCode.Should().Be("USD");
    }

    [Fact]
    public void Constructor_ShouldRoundAmountToTwoDecimalPlaces()
    {
        var money = new Money(100.999m, "EUR");

        money.Amount.Should().Be(101.00m);
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ShouldSucceed()
    {
        var money = new Money(-100.50m, "USD");

        money.Amount.Should().Be(-100.50m);
        money.CurrencyCode.Should().Be("USD");
    }

    [Fact]
    public void AdditionOperator_WithSameCurrency_ShouldReturnSum()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "USD");

        var result = money1 + money2;

        result.Amount.Should().Be(150m);
        result.CurrencyCode.Should().Be("USD");
    }

    [Fact]
    public void AdditionOperator_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "EUR");

        Action act = () => { var result = money1 + money2; };

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot add money with different currencies*");
    }

    [Fact]
    public void SubtractionOperator_WithSameCurrency_ShouldReturnDifference()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(30m, "USD");

        var result = money1 - money2;

        result.Amount.Should().Be(70m);
        result.CurrencyCode.Should().Be("USD");
    }

    [Fact]
    public void SubtractionOperator_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "EUR");

        Action act = () => { var result = money1 - money2; };

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot subtract money with different currencies*");
    }

    [Fact]
    public void MultiplicationOperator_MoneyTimesDecimal_ShouldReturnProduct()
    {
        var money = new Money(100m, "USD");

        var result = money * 1.5m;

        result.Amount.Should().Be(150m);
        result.CurrencyCode.Should().Be("USD");
    }

    [Fact]
    public void MultiplicationOperator_DecimalTimesMoney_ShouldReturnProduct()
    {
        var money = new Money(100m, "USD");

        var result = 2m * money;

        result.Amount.Should().Be(200m);
        result.CurrencyCode.Should().Be("USD");
    }

    [Fact]
    public void DivisionOperator_WithValidDivisor_ShouldReturnQuotient()
    {
        var money = new Money(100m, "USD");

        var result = money / 4m;

        result.Amount.Should().Be(25m);
        result.CurrencyCode.Should().Be("USD");
    }

    [Fact]
    public void DivisionOperator_WithZeroDivisor_ShouldThrowDivideByZeroException()
    {
        var money = new Money(100m, "USD");

        Action act = () => { var result = money / 0m; };

        act.Should().Throw<DivideByZeroException>()
            .WithMessage("Cannot divide money by zero.");
    }

    [Fact]
    public void GreaterThanOperator_WithSameCurrency_ShouldCompareCorrectly()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "USD");

        (money1 > money2).Should().BeTrue();
        (money2 > money1).Should().BeFalse();
    }

    [Fact]
    public void GreaterThanOperator_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "EUR");

        Action act = () => { var result = money1 > money2; };

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot compare money with different currencies*");
    }

    [Fact]
    public void LessThanOperator_WithSameCurrency_ShouldCompareCorrectly()
    {
        var money1 = new Money(50m, "USD");
        var money2 = new Money(100m, "USD");

        (money1 < money2).Should().BeTrue();
        (money2 < money1).Should().BeFalse();
    }

    [Fact]
    public void GreaterThanOrEqualOperator_WithSameCurrency_ShouldCompareCorrectly()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(100m, "USD");
        var money3 = new Money(50m, "USD");

        (money1 >= money2).Should().BeTrue();
        (money1 >= money3).Should().BeTrue();
        (money3 >= money1).Should().BeFalse();
    }

    [Fact]
    public void LessThanOrEqualOperator_WithSameCurrency_ShouldCompareCorrectly()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(100m, "USD");
        var money3 = new Money(150m, "USD");

        (money1 <= money2).Should().BeTrue();
        (money1 <= money3).Should().BeTrue();
        (money3 <= money1).Should().BeFalse();
    }

    [Fact]
    public void Zero_WithCurrency_ShouldReturnZeroWithSpecifiedCurrency()
    {
        var money = Money.Zero("USD");

        money.Amount.Should().Be(0m);
        money.CurrencyCode.Should().Be("USD");
    }

    [Fact]
    public void IsZero_WithZeroAmount_ShouldReturnTrue()
    {
        var money = new Money(0m, "USD");

        money.IsZero().Should().BeTrue();
    }

    [Fact]
    public void IsZero_WithNonZeroAmount_ShouldReturnFalse()
    {
        var money = new Money(0.01m, "USD");

        money.IsZero().Should().BeFalse();
    }

    [Fact]
    public void IsPositive_WithPositiveAmount_ShouldReturnTrue()
    {
        var money = new Money(100m, "USD");

        money.IsPositive().Should().BeTrue();
    }

    [Fact]
    public void IsPositive_WithZeroOrNegativeAmount_ShouldReturnFalse()
    {
        var zero = new Money(0m, "USD");
        var negative = new Money(-100m, "USD");

        zero.IsPositive().Should().BeFalse();
        negative.IsPositive().Should().BeFalse();
    }

    [Fact]
    public void IsNegative_WithNegativeAmount_ShouldReturnTrue()
    {
        var money = new Money(-100m, "USD");

        money.IsNegative().Should().BeTrue();
    }

    [Fact]
    public void IsNegative_WithZeroOrPositiveAmount_ShouldReturnFalse()
    {
        var zero = new Money(0m, "USD");
        var positive = new Money(100m, "USD");

        zero.IsNegative().Should().BeFalse();
        positive.IsNegative().Should().BeFalse();
    }

    [Fact]
    public void Abs_WithNegativeAmount_ShouldReturnPositiveAmount()
    {
        var money = new Money(-100m, "USD");

        var result = money.Abs();

        result.Amount.Should().Be(100m);
        result.CurrencyCode.Should().Be("USD");
    }

    [Fact]
    public void Abs_WithPositiveAmount_ShouldReturnSameAmount()
    {
        var money = new Money(100m, "USD");

        var result = money.Abs();

        result.Amount.Should().Be(100m);
        result.CurrencyCode.Should().Be("USD");
    }

    [Fact]
    public void Negate_ShouldReturnNegatedAmount()
    {
        var positive = new Money(100m, "USD");
        var negative = new Money(-50m, "EUR");

        var negatedPositive = positive.Negate();
        var negatedNegative = negative.Negate();

        negatedPositive.Amount.Should().Be(-100m);
        negatedPositive.CurrencyCode.Should().Be("USD");
        negatedNegative.Amount.Should().Be(50m);
        negatedNegative.CurrencyCode.Should().Be("EUR");
    }

    [Fact]
    public void Allocate_WithValidRatios_ShouldAllocateCorrectly()
    {
        var money = new Money(100m, "USD");

        var result = money.Allocate(50, 30, 20);

        result.Should().HaveCount(3);
        result[0].Amount.Should().Be(50m);
        result[1].Amount.Should().Be(30m);
        result[2].Amount.Should().Be(20m);
        result.Sum(m => m.Amount).Should().Be(100m);
    }

    [Fact]
    public void Allocate_WithRoundingRemainder_ShouldAddRemainderToFirst()
    {
        var money = new Money(100m, "USD");

        var result = money.Allocate(33, 33, 34);

        result.Should().HaveCount(3);
        result.Sum(m => m.Amount).Should().Be(100m);
        result[0].CurrencyCode.Should().Be("USD");
        result[1].CurrencyCode.Should().Be("USD");
        result[2].CurrencyCode.Should().Be("USD");
    }

    [Fact]
    public void Allocate_WithNullRatios_ShouldThrowArgumentException()
    {
        var money = new Money(100m, "USD");

        Action act = () => money.Allocate(null);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Ratios cannot be null or empty.*")
            .WithParameterName("ratios");
    }

    [Fact]
    public void Allocate_WithEmptyRatios_ShouldThrowArgumentException()
    {
        var money = new Money(100m, "USD");

        Action act = () => money.Allocate();

        act.Should().Throw<ArgumentException>()
            .WithMessage("Ratios cannot be null or empty.*")
            .WithParameterName("ratios");
    }

    [Fact]
    public void Allocate_WithNegativeRatios_ShouldThrowArgumentException()
    {
        var money = new Money(100m, "USD");

        Action act = () => money.Allocate(50, -30, 20);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Ratios cannot be negative.*")
            .WithParameterName("ratios");
    }

    [Fact]
    public void Allocate_WithAllZeroRatios_ShouldThrowArgumentException()
    {
        var money = new Money(100m, "USD");

        Action act = () => money.Allocate(0, 0, 0);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Sum of ratios cannot be zero.*")
            .WithParameterName("ratios");
    }

    [Fact]
    public void ConvertTo_WithValidExchangeRate_ShouldConvertCorrectly()
    {
        var money = new Money(100m, "USD");
        var exchangeRate = 0.85m; // USD to EUR

        var result = money.ConvertTo("EUR", exchangeRate);

        result.Amount.Should().Be(85m);
        result.CurrencyCode.Should().Be("EUR");
    }

    [Fact]
    public void ConvertTo_WithSameCurrency_ShouldReturnSameMoney()
    {
        var money = new Money(100m, "USD");

        var result = money.ConvertTo("USD", 1m);

        result.Should().Be(money);
    }

    [Fact]
    public void ConvertTo_WithZeroExchangeRate_ShouldThrowArgumentException()
    {
        var money = new Money(100m, "USD");

        Action act = () => money.ConvertTo("EUR", 0m);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Exchange rate must be positive.*")
            .WithParameterName("exchangeRate");
    }

    [Fact]
    public void ConvertTo_WithNegativeExchangeRate_ShouldThrowArgumentException()
    {
        var money = new Money(100m, "USD");

        Action act = () => money.ConvertTo("EUR", -0.85m);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Exchange rate must be positive.*")
            .WithParameterName("exchangeRate");
    }

    [Fact]
    public void CompareTo_WithSameCurrency_ShouldCompareCorrectly()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "USD");
        var money3 = new Money(100m, "USD");

        money1.CompareTo(money2).Should().BePositive();
        money2.CompareTo(money1).Should().BeNegative();
        money1.CompareTo(money3).Should().Be(0);
    }

    [Fact]
    public void CompareTo_WithNull_ShouldReturnPositive()
    {
        var money = new Money(100m, "USD");

        var result = money.CompareTo(null);

        result.Should().BePositive();
    }

    [Fact]
    public void CompareTo_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "EUR");

        Action act = () => money1.CompareTo(money2);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot compare money with different currencies*");
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        var money = new Money(1234.56m, "USD");

        var result = money.ToString();

        result.Should().Contain("1,234.56");
        result.Should().Contain("USD");
    }

    [Fact]
    public void RecordEquality_WithSameValues_ShouldBeEqual()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(100m, "USD");

        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void RecordEquality_WithDifferentValues_ShouldNotBeEqual()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(100m, "EUR");
        var money3 = new Money(50m, "USD");

        money1.Should().NotBe(money2);
        money1.Should().NotBe(money3);
    }

    [Fact]
    public void Constructor_WithNullCurrencyCode_ShouldThrowArgumentException()
    {
        Action act = () => new Money(100m, null!);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency code cannot be null or empty.*")
            .WithParameterName("currencyCode");
    }

    [Fact]
    public void Constructor_WithEmptyCurrencyCode_ShouldThrowArgumentException()
    {
        Action act = () => new Money(100m, "");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency code cannot be null or empty.*")
            .WithParameterName("currencyCode");
    }

    [Fact]
    public void Constructor_WithWhitespaceCurrencyCode_ShouldThrowArgumentException()
    {
        Action act = () => new Money(100m, "   ");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency code cannot be null or empty.*")
            .WithParameterName("currencyCode");
    }

    [Fact]
    public void Constructor_WithTooShortCurrencyCode_ShouldThrowArgumentException()
    {
        Action act = () => new Money(100m, "US");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency code must be 3 characters (ISO 4217 format).*")
            .WithParameterName("currencyCode");
    }

    [Fact]
    public void Constructor_WithTooLongCurrencyCode_ShouldThrowArgumentException()
    {
        Action act = () => new Money(100m, "USDD");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency code must be 3 characters (ISO 4217 format).*")
            .WithParameterName("currencyCode");
    }

    [Fact]
    public void Constructor_WithNonLetterCurrencyCode_ShouldThrowArgumentException()
    {
        Action act = () => new Money(100m, "U$D");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency code must contain only letters (ISO 4217 format).*")
            .WithParameterName("currencyCode");
    }

    [Fact]
    public void Constructor_WithNumericCurrencyCode_ShouldThrowArgumentException()
    {
        Action act = () => new Money(100m, "123");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency code must contain only letters (ISO 4217 format).*")
            .WithParameterName("currencyCode");
    }

    [Fact]
    public void Constructor_WithMixedCaseCurrencyCode_ShouldNormalizeToUpperCase()
    {
        var money = new Money(100m, "usd");

        money.CurrencyCode.Should().Be("USD");
    }

    [Fact]
    public void Constructor_WithLowercaseCurrencyCode_ShouldNormalizeToUpperCase()
    {
        var money = new Money(100m, "eur");

        money.CurrencyCode.Should().Be("EUR");
    }
}
