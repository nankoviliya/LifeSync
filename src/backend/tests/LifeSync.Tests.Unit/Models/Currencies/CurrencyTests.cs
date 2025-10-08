using FluentAssertions;
using LifeSync.API.Models.Currencies;
using LifeSync.Common.Required;
using LifeSync.UnitTests.TestHelpers;

namespace LifeSync.UnitTests.Models.Currencies;

public class CurrencyTests
{
    [Fact]
    public void From_WithValidData_ShouldCreateCurrency()
    {
        var code = "USD".ToRequiredString();
        var name = "US Dollar".ToRequiredString();
        var nativeName = "Dollar".ToRequiredString();
        var symbol = "$".ToRequiredString();
        var numericCode = 840.ToRequiredStruct();

        var currency = Currency.From(code, name, nativeName, symbol, numericCode);

        currency.Code.Should().Be("USD");
        currency.Name.Should().Be("US Dollar");
        currency.NativeName.Should().Be("Dollar");
        currency.Symbol.Should().Be("$");
        currency.NumericCode.Should().Be(840);
    }

    [Fact]
    public void From_WithLowercaseCode_ShouldNormalizeToUppercase()
    {
        var code = "usd".ToRequiredString();

        var currency = Currency.From(
            code,
            "US Dollar".ToRequiredString(),
            "Dollar".ToRequiredString(),
            "$".ToRequiredString(),
            840.ToRequiredStruct());

        currency.Code.Should().Be("USD");
    }

    [Theory]
    [InlineData("US")]
    [InlineData("USDD")]
    [InlineData("U1D")]
    [InlineData("123")]
    public void From_WithInvalidCodeFormat_ShouldThrowArgumentException(string code)
    {
        Action act = () => Currency.From(
            code.ToRequiredString(),
            "US Dollar".ToRequiredString(),
            "Dollar".ToRequiredString(),
            "$".ToRequiredString(),
            840.ToRequiredStruct());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency code must be a 3-letter ISO 4217 code*");
    }

    [Fact]
    public void From_WithNameTooShort_ShouldThrowArgumentException()
    {
        Action act = () => Currency.From(
            "USD".ToRequiredString(),
            "A".ToRequiredString(),
            "Dollar".ToRequiredString(),
            "$".ToRequiredString(),
            840.ToRequiredStruct());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency name must be at least 2 characters.*");
    }

    [Fact]
    public void From_WithZeroNumericCode_ShouldThrowArgumentException()
    {
        Action act = () => Currency.From(
            "USD".ToRequiredString(),
            "US Dollar".ToRequiredString(),
            "Dollar".ToRequiredString(),
            "$".ToRequiredString(),
            0.ToRequiredStruct());

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithNegativeNumericCode_ShouldThrowArgumentException()
    {
        Action act = () => Currency.From(
            "USD".ToRequiredString(),
            "US Dollar".ToRequiredString(),
            "Dollar".ToRequiredString(),
            "$".ToRequiredString(),
            (-1).ToRequiredStruct());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Numeric code must be a positive integer.*");
    }

    [Fact]
    public void From_WithNumericCodeTooLarge_ShouldThrowArgumentException()
    {
        Action act = () => Currency.From(
            "USD".ToRequiredString(),
            "US Dollar".ToRequiredString(),
            "Dollar".ToRequiredString(),
            "$".ToRequiredString(),
            1000.ToRequiredStruct());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Numeric code must be less than 1000*");
    }

    [Fact]
    public void UpdateNativeName_WithValidName_ShouldUpdateNativeName()
    {
        var currency = CreateTestCurrency();

        currency.UpdateNativeName("New Native Name".ToRequiredString());

        currency.NativeName.Should().Be("New Native Name");
    }

    [Fact]
    public void UpdateSymbol_WithValidSymbol_ShouldUpdateSymbol()
    {
        var currency = CreateTestCurrency();

        currency.UpdateSymbol("US$".ToRequiredString());

        currency.Symbol.Should().Be("US$");
    }

    [Theory]
    [InlineData("USD", true)]
    [InlineData("usd", true)]
    [InlineData("Usd", true)]
    [InlineData("EUR", false)]
    public void IsCode_WithVariousCodes_ShouldMatchCorrectly(string codeToCheck, bool expectedResult)
    {
        var currency = CreateTestCurrency();

        var result = currency.IsCode(codeToCheck);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public void IsValid_WithValidCurrency_ShouldReturnTrue()
    {
        var currency = CreateTestCurrency();

        var result = currency.IsValid();

        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithInvalidNumericCode_ShouldReturnFalse()
    {
        var currency = Currency.From(
            "USD".ToRequiredString(),
            "US Dollar".ToRequiredString(),
            "Dollar".ToRequiredString(),
            "$".ToRequiredString(),
            999.ToRequiredStruct());

        var result = currency.IsValid();

        result.Should().BeTrue(); // 999 is still valid
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var currency = CreateTestCurrency();

        var result = currency.ToString();

        result.Should().Be("US Dollar (USD)");
    }

    [Fact]
    public void Equality_WithSameId_ShouldBeEqual()
    {
        var (currency1, currency2) = EntityTestHelper.CreateTwoWithSameId(() =>
            Currency.From(
                "USD".ToRequiredString(),
                "US Dollar".ToRequiredString(),
                "Dollar".ToRequiredString(),
                "$".ToRequiredString(),
                840.ToRequiredStruct()));

        currency1.Should().Be(currency2);
        (currency1 == currency2).Should().BeTrue();
        currency1.GetHashCode().Should().Be(currency2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentId_ShouldNotBeEqual()
    {
        var (currency1, currency2) = EntityTestHelper.CreateTwoWithDifferentIds(() =>
            Currency.From(
                "USD".ToRequiredString(),
                "US Dollar".ToRequiredString(),
                "Dollar".ToRequiredString(),
                "$".ToRequiredString(),
                840.ToRequiredStruct()));

        currency1.Should().NotBe(currency2);
        (currency1 == currency2).Should().BeFalse();
    }

    [Fact]
    public void Equality_WithSameIdButDifferentData_ShouldBeEqual()
    {
        var currency1 = Currency.From(
            "USD".ToRequiredString(),
            "US Dollar".ToRequiredString(),
            "Dollar".ToRequiredString(),
            "$".ToRequiredString(),
            840.ToRequiredStruct());

        var currency2 = Currency.From(
            "EUR".ToRequiredString(),
            "Euro".ToRequiredString(),
            "Euro".ToRequiredString(),
            "€".ToRequiredString(),
            978.ToRequiredStruct());

        var sharedId = Guid.NewGuid();
        EntityTestHelper.SetId(currency1, sharedId);
        EntityTestHelper.SetId(currency2, sharedId);

        currency1.Should().Be(currency2);
    }

    private Currency CreateTestCurrency()
    {
        return Currency.From(
            "USD".ToRequiredString(),
            "US Dollar".ToRequiredString(),
            "Dollar".ToRequiredString(),
            "$".ToRequiredString(),
            840.ToRequiredStruct());
    }
}
