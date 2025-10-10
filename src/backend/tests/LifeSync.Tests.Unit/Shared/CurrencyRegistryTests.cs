using LifeSync.API.Shared;

namespace LifeSync.UnitTests.Shared;

public class CurrencyRegistryTests
{
    [Fact]
    public void GetByCode_ShouldReturnCurrency_WhenCodeIsValid()
    {
        CurrencyInfo? result = CurrencyRegistry.GetByCode("USD");

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
        CurrencyInfo? result = CurrencyRegistry.GetByCode(input);

        Assert.NotNull(result);
        Assert.Equal(expectedCode, result.Code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetByCode_ShouldReturnNull_WhenCodeIsNullOrWhitespace(string code)
    {
        CurrencyInfo? result = CurrencyRegistry.GetByCode(code);

        Assert.Null(result);
    }

    [Theory]
    [InlineData("XXX")]
    [InlineData("INVALID")]
    [InlineData("123")]
    public void GetByCode_ShouldReturnNull_WhenCodeIsNotSupported(string code)
    {
        CurrencyInfo? result = CurrencyRegistry.GetByCode(code);

        Assert.Null(result);
    }

    [Fact]
    public void IsSupported_ShouldReturnTrue_WhenCodeIsSupported()
    {
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
    public void IsSupported_ShouldBeCaseInsensitive(string code) =>
        Assert.True(CurrencyRegistry.IsSupported(code));

    [Theory]
    [InlineData("XXX")]
    [InlineData("INVALID")]
    [InlineData("123")]
    public void IsSupported_ShouldReturnFalse_WhenCodeIsNotSupported(string code) =>
        Assert.False(CurrencyRegistry.IsSupported(code));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsSupported_ShouldReturnFalse_WhenCodeIsNullOrWhitespace(string code) =>
        Assert.False(CurrencyRegistry.IsSupported(code));

    [Fact]
    public void All_ShouldReturnAllSupportedCurrencies()
    {
        IReadOnlyCollection<CurrencyInfo> allCurrencies = CurrencyRegistry.All;

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
        string result = CurrencyRegistry.GetSupportedCodesString();

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
        CurrencyInfo usd = CurrencyRegistry.USD;
        CurrencyInfo eur = CurrencyRegistry.EUR;
        CurrencyInfo bgn = CurrencyRegistry.BGN;
        CurrencyInfo uah = CurrencyRegistry.UAH;
        CurrencyInfo rub = CurrencyRegistry.RUB;

        Assert.Equal("USD", usd.Code);
        Assert.Equal("US Dollar", usd.Name);
        Assert.Equal("United States Dollar", usd.NativeName);
        Assert.Equal("$", usd.Symbol);

        Assert.Equal("EUR", eur.Code);
        Assert.Equal("Euro", eur.Name);
        Assert.Equal("Euro", eur.NativeName);
        Assert.Equal("€", eur.Symbol);

        Assert.Equal("BGN", bgn.Code);
        Assert.Equal("Bulgarian Lev", bgn.Name);
        Assert.Equal("Български лев", bgn.NativeName);
        Assert.Equal("лв", bgn.Symbol);

        Assert.Equal("UAH", uah.Code);
        Assert.Equal("Ukrainian Hryvnia", uah.Name);
        Assert.Equal("Українська гривня", uah.NativeName);
        Assert.Equal("₴", uah.Symbol);

        Assert.Equal("RUB", rub.Code);
        Assert.Equal("Russian Ruble", rub.Name);
        Assert.Equal("Российский рубль", rub.NativeName);
        Assert.Equal("₽", rub.Symbol);
    }

    [Fact]
    public void CurrencyInfo_ToString_ShouldReturnNameAndCode()
    {
        CurrencyInfo usd = CurrencyRegistry.USD;

        string result = usd.ToString();

        Assert.Equal("US Dollar (USD)", result);
    }

    [Fact]
    public void GetByCode_ShouldReturnSameInstance_WhenCalledMultipleTimes()
    {
        CurrencyInfo? first = CurrencyRegistry.GetByCode("USD");
        CurrencyInfo? second = CurrencyRegistry.GetByCode("USD");

        Assert.Same(first, second);
    }

    [Fact]
    public void All_ShouldBeReadOnly()
    {
        IReadOnlyCollection<CurrencyInfo> allCurrencies = CurrencyRegistry.All;

        Assert.IsAssignableFrom<IReadOnlyCollection<CurrencyInfo>>(allCurrencies);
    }
}