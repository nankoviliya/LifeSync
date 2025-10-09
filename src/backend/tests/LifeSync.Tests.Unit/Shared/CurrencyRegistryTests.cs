using LifeSync.API.Shared;
using Xunit;

namespace LifeSync.Tests.Unit.Shared;

public class CurrencyRegistryTests
{
    [Fact]
    public void GetByCode_ShouldReturnCurrency_WhenCodeIsValid()
    {
        // Act
        var result = CurrencyRegistry.GetByCode("USD");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("USD", result.Code);
        Assert.Equal("US Dollar", result.Name);
        Assert.Equal("$", result.Symbol);
    }

    [Theory]
    [InlineData("usd", "USD")]
    [InlineData("eur", "EUR")]
    [InlineData("bgn", "BGN")]
    [InlineData("Uah", "UAH")]
    [InlineData("RuB", "RUB")]
    public void GetByCode_ShouldBeCaseInsensitive(string input, string expectedCode)
    {
        // Act
        var result = CurrencyRegistry.GetByCode(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCode, result.Code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetByCode_ShouldReturnNull_WhenCodeIsNullOrWhitespace(string code)
    {
        // Act
        var result = CurrencyRegistry.GetByCode(code);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("XXX")]
    [InlineData("INVALID")]
    [InlineData("123")]
    public void GetByCode_ShouldReturnNull_WhenCodeIsNotSupported(string code)
    {
        // Act
        var result = CurrencyRegistry.GetByCode(code);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void IsSupported_ShouldReturnTrue_WhenCodeIsSupported()
    {
        // Act & Assert
        Assert.True(CurrencyRegistry.IsSupported("USD"));
        Assert.True(CurrencyRegistry.IsSupported("EUR"));
        Assert.True(CurrencyRegistry.IsSupported("BGN"));
        Assert.True(CurrencyRegistry.IsSupported("UAH"));
        Assert.True(CurrencyRegistry.IsSupported("RUB"));
    }

    [Theory]
    [InlineData("usd")]
    [InlineData("eur")]
    [InlineData("bgn")]
    public void IsSupported_ShouldBeCaseInsensitive(string code)
    {
        // Act & Assert
        Assert.True(CurrencyRegistry.IsSupported(code));
    }

    [Theory]
    [InlineData("XXX")]
    [InlineData("INVALID")]
    [InlineData("123")]
    public void IsSupported_ShouldReturnFalse_WhenCodeIsNotSupported(string code)
    {
        // Act & Assert
        Assert.False(CurrencyRegistry.IsSupported(code));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsSupported_ShouldReturnFalse_WhenCodeIsNullOrWhitespace(string code)
    {
        // Act & Assert
        Assert.False(CurrencyRegistry.IsSupported(code));
    }

    [Fact]
    public void All_ShouldReturnAllSupportedCurrencies()
    {
        // Act
        var allCurrencies = CurrencyRegistry.All;

        // Assert
        Assert.NotNull(allCurrencies);
        Assert.Equal(5, allCurrencies.Count);
        Assert.Contains(allCurrencies, c => c.Code == "USD");
        Assert.Contains(allCurrencies, c => c.Code == "EUR");
        Assert.Contains(allCurrencies, c => c.Code == "BGN");
        Assert.Contains(allCurrencies, c => c.Code == "UAH");
        Assert.Contains(allCurrencies, c => c.Code == "RUB");
    }

    [Fact]
    public void GetSupportedCodesString_ShouldReturnCommaSeparatedCodes()
    {
        // Act
        var result = CurrencyRegistry.GetSupportedCodesString();

        // Assert
        Assert.NotNull(result);
        Assert.Contains("USD", result);
        Assert.Contains("EUR", result);
        Assert.Contains("BGN", result);
        Assert.Contains("UAH", result);
        Assert.Contains("RUB", result);
    }

    [Fact]
    public void CurrencyInfo_Properties_ShouldBeCorrect()
    {
        // Arrange
        var usd = CurrencyRegistry.USD;
        var eur = CurrencyRegistry.EUR;
        var bgn = CurrencyRegistry.BGN;
        var uah = CurrencyRegistry.UAH;
        var rub = CurrencyRegistry.RUB;

        // Assert USD
        Assert.Equal("USD", usd.Code);
        Assert.Equal("US Dollar", usd.Name);
        Assert.Equal("United States Dollar", usd.NativeName);
        Assert.Equal("$", usd.Symbol);

        // Assert EUR
        Assert.Equal("EUR", eur.Code);
        Assert.Equal("Euro", eur.Name);
        Assert.Equal("Euro", eur.NativeName);
        Assert.Equal("€", eur.Symbol);

        // Assert BGN
        Assert.Equal("BGN", bgn.Code);
        Assert.Equal("Bulgarian Lev", bgn.Name);
        Assert.Equal("Български лев", bgn.NativeName);
        Assert.Equal("лв", bgn.Symbol);

        // Assert UAH
        Assert.Equal("UAH", uah.Code);
        Assert.Equal("Ukrainian Hryvnia", uah.Name);
        Assert.Equal("Українська гривня", uah.NativeName);
        Assert.Equal("₴", uah.Symbol);

        // Assert RUB
        Assert.Equal("RUB", rub.Code);
        Assert.Equal("Russian Ruble", rub.Name);
        Assert.Equal("Российский рубль", rub.NativeName);
        Assert.Equal("₽", rub.Symbol);
    }

    [Fact]
    public void CurrencyInfo_ToString_ShouldReturnNameAndCode()
    {
        // Arrange
        var usd = CurrencyRegistry.USD;

        // Act
        var result = usd.ToString();

        // Assert
        Assert.Equal("US Dollar (USD)", result);
    }

    [Fact]
    public void GetByCode_ShouldReturnSameInstance_WhenCalledMultipleTimes()
    {
        // Act
        var first = CurrencyRegistry.GetByCode("USD");
        var second = CurrencyRegistry.GetByCode("USD");

        // Assert
        Assert.Same(first, second);
    }

    [Fact]
    public void All_ShouldBeReadOnly()
    {
        // Act
        var allCurrencies = CurrencyRegistry.All;

        // Assert
        Assert.IsAssignableFrom<IReadOnlyCollection<CurrencyInfo>>(allCurrencies);
    }
}
