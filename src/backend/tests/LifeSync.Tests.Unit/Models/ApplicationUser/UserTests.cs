using FluentAssertions;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Expenses;
using LifeSync.API.Models.Incomes;
using LifeSync.API.Shared;
using LifeSync.Common.Required;

namespace LifeSync.Tests.Unit.Models.ApplicationUser;

public class UserTests
{
    private readonly Guid _testLanguageId = Guid.NewGuid();

    [Fact]
    public void From_WithValidData_ShouldCreateUser()
    {
        var userName = "testuser".ToRequiredString();
        var email = "test@example.com".ToRequiredString();
        var firstName = "John".ToRequiredString();
        var lastName = "Doe".ToRequiredString();
        var balance = new Money(100m, "USD").ToRequiredReference();
        var currencyPreference = "USD".ToRequiredString();
        var languageId = _testLanguageId.ToRequiredStruct();

        var user = User.From(userName, email, firstName, lastName, balance, currencyPreference, languageId);

        user.UserName.Should().Be("testuser");
        user.Email.Should().Be("test@example.com");
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.Balance.Amount.Should().Be(100m);
        user.CurrencyPreference.Should().Be("USD");
        user.LanguageId.Should().Be(_testLanguageId);
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("a")]
    public void From_WithUserNameTooShort_ShouldThrowArgumentException(string userName)
    {
        Action act = () => User.From(
            userName.ToRequiredString(),
            "test@example.com".ToRequiredString(),
            "John".ToRequiredString(),
            "Doe".ToRequiredString(),
            new Money(100m, "USD").ToRequiredReference(),
            "USD".ToRequiredString(),
            _testLanguageId.ToRequiredStruct());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Username must be at least 3 characters.*");
    }

    [Fact]
    public void From_WithUserNameTooLong_ShouldThrowArgumentException()
    {
        var longUserName = new string('a', 51);

        Action act = () => User.From(
            longUserName.ToRequiredString(),
            "test@example.com".ToRequiredString(),
            "John".ToRequiredString(),
            "Doe".ToRequiredString(),
            new Money(100m, "USD").ToRequiredReference(),
            "USD".ToRequiredString(),
            _testLanguageId.ToRequiredStruct());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Username cannot exceed 50 characters.*");
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [InlineData("test example.com")]
    public void From_WithInvalidEmail_ShouldThrowArgumentException(string email)
    {
        Action act = () => User.From(
            "testuser".ToRequiredString(),
            email.ToRequiredString(),
            "John".ToRequiredString(),
            "Doe".ToRequiredString(),
            new Money(100m, "USD").ToRequiredReference(),
            "USD".ToRequiredString(),
            _testLanguageId.ToRequiredStruct());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Email format is invalid.*");
    }

    [Fact]
    public void From_WithEmptyLanguageId_ShouldThrowArgumentException()
    {
        Action act = () => User.From(
            "testuser".ToRequiredString(),
            "test@example.com".ToRequiredString(),
            "John".ToRequiredString(),
            "Doe".ToRequiredString(),
            new Money(100m, "USD").ToRequiredReference(),
            "USD".ToRequiredString(),
            Guid.Empty.ToRequiredStruct());

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetFullName_ShouldReturnFirstAndLastName()
    {
        var user = CreateTestUser("John", "Doe");

        var fullName = user.GetFullName();

        fullName.Should().Be("John Doe");
    }

    [Fact]
    public void UpdateFirstName_WithValidName_ShouldUpdateFirstName()
    {
        var user = CreateTestUser("John", "Doe");

        user.UpdateFirstName("Jane".ToRequiredString());

        user.FirstName.Should().Be("Jane");
    }

    [Fact]
    public void UpdateFirstName_WithInvalidName_ShouldThrowArgumentException()
    {
        var user = CreateTestUser("John", "Doe");
        var longName = new string('a', 101);

        Action act = () => user.UpdateFirstName(longName.ToRequiredString());

        act.Should().Throw<ArgumentException>()
            .WithMessage("First name cannot exceed 100 characters.*");
    }

    [Fact]
    public void UpdateLastName_WithValidName_ShouldUpdateLastName()
    {
        var user = CreateTestUser("John", "Doe");

        user.UpdateLastName("Smith".ToRequiredString());

        user.LastName.Should().Be("Smith");
    }

    [Fact]
    public void UpdateFullName_WithValidNames_ShouldUpdateBothNames()
    {
        var user = CreateTestUser("John", "Doe");

        user.UpdateFullName("Jane".ToRequiredString(), "Smith".ToRequiredString());

        user.FirstName.Should().Be("Jane");
        user.LastName.Should().Be("Smith");
    }

    [Fact]
    public void UpdateLanguage_WithValidLanguageId_ShouldUpdateLanguage()
    {
        var user = CreateTestUser();
        var newLanguageId = Guid.NewGuid();

        user.UpdateLanguage(newLanguageId.ToRequiredStruct());

        user.LanguageId.Should().Be(newLanguageId);
    }

    [Fact]
    public void UpdateCurrencyPreference_WithSameCurrency_ShouldNotChangeBalance()
    {
        var user = CreateTestUser();
        var originalBalance = user.Balance;

        user.UpdateCurrencyPreference("USD".ToRequiredString(), 1.0m);

        user.Balance.Should().Be(originalBalance);
        user.CurrencyPreference.Should().Be("USD");
    }

    [Fact]
    public void UpdateCurrencyPreference_WithDifferentCurrency_ShouldConvertBalance()
    {
        var user = CreateTestUser();
        var exchangeRate = 0.85m;

        user.UpdateCurrencyPreference("EUR".ToRequiredString(), exchangeRate);

        user.Balance.Amount.Should().Be(85m);
        user.Balance.CurrencyCode.Should().Be("EUR");
        user.CurrencyPreference.Should().Be("EUR");
    }

    [Fact]
    public void UpdateCurrencyPreference_WithNegativeExchangeRate_ShouldThrowArgumentException()
    {
        var user = CreateTestUser();

        Action act = () => user.UpdateCurrencyPreference("EUR".ToRequiredString(), -0.85m);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Exchange rate must be positive.*");
    }

    [Fact]
    public void UpdateBalance_WithMatchingCurrency_ShouldUpdateBalance()
    {
        var user = CreateTestUser();
        var newBalance = new Money(200m, "USD");

        user.UpdateBalance(newBalance);

        user.Balance.Should().Be(newBalance);
    }

    [Fact]
    public void UpdateBalance_WithDifferentCurrency_ShouldThrowArgumentException()
    {
        var user = CreateTestUser();
        var newBalance = new Money(200m, "EUR");

        Action act = () => user.UpdateBalance(newBalance);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency mismatch*");
    }

    [Fact]
    public void Deposit_WithValidAmount_ShouldIncreaseBalance()
    {
        var user = CreateTestUser();
        var depositAmount = new Money(50m, "USD");

        user.Deposit(depositAmount);

        user.Balance.Amount.Should().Be(150m);
    }

    [Fact]
    public void Deposit_WithZeroAmount_ShouldThrowArgumentException()
    {
        var user = CreateTestUser();
        var depositAmount = new Money(0m, "USD");

        Action act = () => user.Deposit(depositAmount);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Cannot deposit zero amount*");
    }

    [Fact]
    public void Deposit_WithNegativeAmount_ShouldThrowArgumentException()
    {
        var user = CreateTestUser();
        var depositAmount = new Money(-50m, "USD");

        Action act = () => user.Deposit(depositAmount);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Cannot deposit negative amount*");
    }

    [Fact]
    public void Deposit_WithDifferentCurrency_ShouldThrowArgumentException()
    {
        var user = CreateTestUser();
        var depositAmount = new Money(50m, "EUR");

        Action act = () => user.Deposit(depositAmount);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency mismatch*");
    }

    [Fact]
    public void Withdraw_WithValidAmount_ShouldDecreaseBalance()
    {
        var user = CreateTestUser();
        var withdrawAmount = new Money(30m, "USD");

        user.Withdraw(withdrawAmount);

        user.Balance.Amount.Should().Be(70m);
    }

    [Fact]
    public void Withdraw_WithAmountExceedingBalance_ShouldThrowInvalidOperationException()
    {
        var user = CreateTestUser();
        var withdrawAmount = new Money(200m, "USD");

        Action act = () => user.Withdraw(withdrawAmount);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Insufficient balance*");
    }

    [Fact]
    public void Withdraw_WithZeroAmount_ShouldThrowArgumentException()
    {
        var user = CreateTestUser();
        var withdrawAmount = new Money(0m, "USD");

        Action act = () => user.Withdraw(withdrawAmount);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Cannot withdraw zero amount*");
    }

    [Fact]
    public void Withdraw_WithNegativeAmount_ShouldThrowArgumentException()
    {
        var user = CreateTestUser();
        var withdrawAmount = new Money(-50m, "USD");

        Action act = () => user.Withdraw(withdrawAmount);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Cannot withdraw negative amount*");
    }

    [Fact]
    public void HasSufficientBalance_WithSufficientAmount_ShouldReturnTrue()
    {
        var user = CreateTestUser();
        var amount = new Money(50m, "USD");

        var result = user.HasSufficientBalance(amount);

        result.Should().BeTrue();
    }

    [Fact]
    public void HasSufficientBalance_WithInsufficientAmount_ShouldReturnFalse()
    {
        var user = CreateTestUser();
        var amount = new Money(200m, "USD");

        var result = user.HasSufficientBalance(amount);

        result.Should().BeFalse();
    }

    [Fact]
    public void HasSufficientBalance_WithDifferentCurrency_ShouldReturnFalse()
    {
        var user = CreateTestUser();
        var amount = new Money(50m, "EUR");

        var result = user.HasSufficientBalance(amount);

        result.Should().BeFalse();
    }

    [Fact]
    public void GetTotalIncome_WithIncomeTransactionsInRange_ShouldReturnTotal()
    {
        var user = CreateTestUser();
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 12, 31);

        var income1 = IncomeTransaction.From(
            new Money(100m, "USD").ToRequiredReference(),
            new DateTime(2024, 6, 1).ToRequiredStruct(),
            "Salary".ToRequiredString(),
            "user1".ToRequiredString());

        var income2 = IncomeTransaction.From(
            new Money(50m, "USD").ToRequiredReference(),
            new DateTime(2024, 7, 1).ToRequiredStruct(),
            "Bonus".ToRequiredString(),
            "user1".ToRequiredString());

        user.IncomeTransactions.Add(income1);
        user.IncomeTransactions.Add(income2);

        var total = user.GetTotalIncome(startDate, endDate);

        total.Amount.Should().Be(150m);
    }

    [Fact]
    public void GetTotalExpenses_WithExpenseTransactionsInRange_ShouldReturnTotal()
    {
        var user = CreateTestUser();
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 12, 31);

        var expense1 = ExpenseTransaction.From(
            new Money(30m, "USD").ToRequiredReference(),
            new DateTime(2024, 6, 1).ToRequiredStruct(),
            "Groceries".ToRequiredString(),
            ExpenseType.Needs,
            "user1".ToRequiredString());

        var expense2 = ExpenseTransaction.From(
            new Money(20m, "USD").ToRequiredReference(),
            new DateTime(2024, 7, 1).ToRequiredStruct(),
            "Entertainment".ToRequiredString(),
            ExpenseType.Wants,
            "user1".ToRequiredString());

        user.ExpenseTransactions.Add(expense1);
        user.ExpenseTransactions.Add(expense2);

        var total = user.GetTotalExpenses(startDate, endDate);

        total.Amount.Should().Be(50m);
    }

    [Fact]
    public void GetNetIncome_WithIncomeAndExpenses_ShouldReturnDifference()
    {
        var user = CreateTestUser();
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 12, 31);

        var income = IncomeTransaction.From(
            new Money(100m, "USD").ToRequiredReference(),
            new DateTime(2024, 6, 1).ToRequiredStruct(),
            "Salary".ToRequiredString(),
            "user1".ToRequiredString());

        var expense = ExpenseTransaction.From(
            new Money(30m, "USD").ToRequiredReference(),
            new DateTime(2024, 6, 15).ToRequiredStruct(),
            "Rent".ToRequiredString(),
            ExpenseType.Needs,
            "user1".ToRequiredString());

        user.IncomeTransactions.Add(income);
        user.ExpenseTransactions.Add(expense);

        var netIncome = user.GetNetIncome(startDate, endDate);

        netIncome.Amount.Should().Be(70m);
    }

    [Fact]
    public void GetExpensesByType_WithSpecificType_ShouldReturnFilteredTotal()
    {
        var user = CreateTestUser();
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 12, 31);

        var needsExpense = ExpenseTransaction.From(
            new Money(100m, "USD").ToRequiredReference(),
            new DateTime(2024, 6, 1).ToRequiredStruct(),
            "Rent".ToRequiredString(),
            ExpenseType.Needs,
            "user1".ToRequiredString());

        var wantsExpense = ExpenseTransaction.From(
            new Money(50m, "USD").ToRequiredReference(),
            new DateTime(2024, 6, 15).ToRequiredStruct(),
            "Entertainment".ToRequiredString(),
            ExpenseType.Wants,
            "user1".ToRequiredString());

        user.ExpenseTransactions.Add(needsExpense);
        user.ExpenseTransactions.Add(wantsExpense);

        var needsTotal = user.GetExpensesByType(ExpenseType.Needs, startDate, endDate);

        needsTotal.Amount.Should().Be(100m);
    }

    private User CreateTestUser(string firstName = "John", string lastName = "Doe")
    {
        return User.From(
            "testuser".ToRequiredString(),
            "test@example.com".ToRequiredString(),
            firstName.ToRequiredString(),
            lastName.ToRequiredString(),
            new Money(100m, "USD").ToRequiredReference(),
            "USD".ToRequiredString(),
            _testLanguageId.ToRequiredStruct());
    }
}
