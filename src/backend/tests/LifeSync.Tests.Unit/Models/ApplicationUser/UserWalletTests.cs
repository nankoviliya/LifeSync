using FluentAssertions;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Shared;
using LifeSync.Common.Required;
using LifeSync.UnitTests.TestHelpers;

namespace LifeSync.UnitTests.Models.ApplicationUser;

public class UserWalletTests
{
    [Fact]
    public void From_WithValidData_ShouldCreateWallet()
    {
        var userId = "user123".ToRequiredString();
        var name = "Savings".ToRequiredString();
        var initialBalance = new Money(100m, "USD").ToRequiredReference();

        var wallet = UserWallet.From(userId, name, initialBalance);

        wallet.UserId.Should().Be("user123");
        wallet.Name.Should().Be("Savings");
        wallet.Balance.Amount.Should().Be(100m);
        wallet.Balance.CurrencyCode.Should().Be("USD");
    }

    [Fact]
    public void From_WithNegativeInitialBalance_ShouldThrowArgumentException()
    {
        var userId = "user123".ToRequiredString();
        var name = "Savings".ToRequiredString();
        var initialBalance = new Money(-100m, "USD").ToRequiredReference();

        Action act = () => UserWallet.From(userId, name, initialBalance);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Initial balance cannot be negative.*");
    }

    [Fact]
    public void From_WithNameTooShort_ShouldThrowArgumentException()
    {
        Action act = () => UserWallet.From(
            "user123".ToRequiredString(),
            "A".ToRequiredString(),
            new Money(100m, "USD").ToRequiredReference());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Wallet name must be at least 2 characters.*");
    }

    [Fact]
    public void From_WithNameTooLong_ShouldThrowArgumentException()
    {
        var longName = new string('a', 101);

        Action act = () => UserWallet.From(
            "user123".ToRequiredString(),
            longName.ToRequiredString(),
            new Money(100m, "USD").ToRequiredReference());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Wallet name cannot exceed 100 characters.*");
    }

    [Fact]
    public void Rename_WithValidName_ShouldUpdateName()
    {
        var wallet = CreateTestWallet("Old Name");

        wallet.Rename("New Name".ToRequiredString());

        wallet.Name.Should().Be("New Name");
    }

    [Fact]
    public void Deposit_WithValidAmount_ShouldIncreaseBalance()
    {
        var wallet = CreateTestWallet();
        var depositAmount = new Money(50m, "USD");

        wallet.Deposit(depositAmount);

        wallet.Balance.Amount.Should().Be(150m);
    }

    [Fact]
    public void Deposit_WithZeroAmount_ShouldThrowArgumentException()
    {
        var wallet = CreateTestWallet();
        var depositAmount = new Money(0m, "USD");

        Action act = () => wallet.Deposit(depositAmount);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Cannot deposit zero amount.*");
    }

    [Fact]
    public void Deposit_WithNegativeAmount_ShouldThrowArgumentException()
    {
        var wallet = CreateTestWallet();
        var depositAmount = new Money(-50m, "USD");

        Action act = () => wallet.Deposit(depositAmount);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Cannot deposit negative amount.*");
    }

    [Fact]
    public void Deposit_WithDifferentCurrency_ShouldThrowArgumentException()
    {
        var wallet = CreateTestWallet();
        var depositAmount = new Money(50m, "EUR");

        Action act = () => wallet.Deposit(depositAmount);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency mismatch*");
    }

    [Fact]
    public void Withdraw_WithValidAmount_ShouldDecreaseBalance()
    {
        var wallet = CreateTestWallet();
        var withdrawAmount = new Money(30m, "USD");

        wallet.Withdraw(withdrawAmount);

        wallet.Balance.Amount.Should().Be(70m);
    }

    [Fact]
    public void Withdraw_WithAmountExceedingBalance_ShouldThrowInvalidOperationException()
    {
        var wallet = CreateTestWallet();
        var withdrawAmount = new Money(200m, "USD");

        Action act = () => wallet.Withdraw(withdrawAmount);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Insufficient balance*");
    }

    [Fact]
    public void Withdraw_WithZeroAmount_ShouldThrowArgumentException()
    {
        var wallet = CreateTestWallet();
        var withdrawAmount = new Money(0m, "USD");

        Action act = () => wallet.Withdraw(withdrawAmount);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Cannot withdraw zero amount.*");
    }

    [Fact]
    public void TransferTo_WithValidAmount_ShouldTransferBetweenWallets()
    {
        var sourceWallet = UserWallet.From(
            "user123".ToRequiredString(),
            "Source".ToRequiredString(),
            new Money(100m, "USD").ToRequiredReference());
        EntityTestHelper.SetUniqueId(sourceWallet);

        var targetWallet = UserWallet.From(
            "user456".ToRequiredString(),
            "Target".ToRequiredString(),
            new Money(50m, "USD").ToRequiredReference());
        EntityTestHelper.SetUniqueId(targetWallet);

        var transferAmount = new Money(30m, "USD");

        sourceWallet.TransferTo(targetWallet, transferAmount);

        sourceWallet.Balance.Amount.Should().Be(70m);
        targetWallet.Balance.Amount.Should().Be(80m);
    }

    [Fact]
    public void TransferTo_ToSameWallet_ShouldThrowInvalidOperationException()
    {
        var wallet = CreateTestWallet();
        var transferAmount = new Money(30m, "USD");

        Action act = () => wallet.TransferTo(wallet, transferAmount);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot transfer to the same wallet.*");
    }

    [Fact]
    public void TransferTo_ToDeletedWallet_ShouldThrowInvalidOperationException()
    {
        var sourceWallet = UserWallet.From(
            "user123".ToRequiredString(),
            "Source".ToRequiredString(),
            new Money(100m, "USD").ToRequiredReference());
        EntityTestHelper.SetUniqueId(sourceWallet);

        var targetWallet = UserWallet.From(
            "user456".ToRequiredString(),
            "Target".ToRequiredString(),
            new Money(50m, "USD").ToRequiredReference());
        EntityTestHelper.SetUniqueId(targetWallet);

        targetWallet.MarkAsDeleted();
        var transferAmount = new Money(30m, "USD");

        Action act = () => sourceWallet.TransferTo(targetWallet, transferAmount);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot transfer to a deleted wallet.*");
    }

    [Fact]
    public void TransferTo_WithDifferentCurrencies_ShouldThrowArgumentException()
    {
        var sourceWallet = UserWallet.From(
            "user123".ToRequiredString(),
            "Source".ToRequiredString(),
            new Money(100m, "USD").ToRequiredReference());
        EntityTestHelper.SetUniqueId(sourceWallet);

        var targetWallet = UserWallet.From(
            "user456".ToRequiredString(),
            "Target".ToRequiredString(),
            new Money(50m, "EUR").ToRequiredReference());
        EntityTestHelper.SetUniqueId(targetWallet);

        var transferAmount = new Money(30m, "USD");

        Action act = () => sourceWallet.TransferTo(targetWallet, transferAmount);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency mismatch*");
    }

    [Fact]
    public void TransferTo_WithInsufficientBalance_ShouldThrowInvalidOperationException()
    {
        var sourceWallet = UserWallet.From(
            "user123".ToRequiredString(),
            "Source".ToRequiredString(),
            new Money(20m, "USD").ToRequiredReference());
        EntityTestHelper.SetUniqueId(sourceWallet);

        var targetWallet = UserWallet.From(
            "user456".ToRequiredString(),
            "Target".ToRequiredString(),
            new Money(50m, "USD").ToRequiredReference());
        EntityTestHelper.SetUniqueId(targetWallet);

        var transferAmount = new Money(30m, "USD");

        Action act = () => sourceWallet.TransferTo(targetWallet, transferAmount);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Insufficient balance*");
    }

    [Fact]
    public void SetBalance_WithMatchingCurrency_ShouldUpdateBalance()
    {
        var wallet = CreateTestWallet();
        var newBalance = new Money(200m, "USD");

        wallet.SetBalance(newBalance);

        wallet.Balance.Amount.Should().Be(200m);
    }

    [Fact]
    public void SetBalance_WithDifferentCurrency_ShouldThrowArgumentException()
    {
        var wallet = CreateTestWallet();
        var newBalance = new Money(200m, "EUR");

        Action act = () => wallet.SetBalance(newBalance);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency mismatch*");
    }

    [Fact]
    public void HasSufficientBalance_WithSufficientAmount_ShouldReturnTrue()
    {
        var wallet = CreateTestWallet();
        var amount = new Money(50m, "USD");

        var result = wallet.HasSufficientBalance(amount);

        result.Should().BeTrue();
    }

    [Fact]
    public void HasSufficientBalance_WithInsufficientAmount_ShouldReturnFalse()
    {
        var wallet = CreateTestWallet();
        var amount = new Money(200m, "USD");

        var result = wallet.HasSufficientBalance(amount);

        result.Should().BeFalse();
    }

    [Fact]
    public void HasSufficientBalance_WithDifferentCurrency_ShouldReturnFalse()
    {
        var wallet = CreateTestWallet();
        var amount = new Money(50m, "EUR");

        var result = wallet.HasSufficientBalance(amount);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsEmpty_WithZeroBalance_ShouldReturnTrue()
    {
        var wallet = CreateTestWallet("Empty Wallet", 0m);

        var result = wallet.IsEmpty();

        result.Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_WithNonZeroBalance_ShouldReturnFalse()
    {
        var wallet = CreateTestWallet();

        var result = wallet.IsEmpty();

        result.Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var wallet = CreateTestWallet("My Wallet", 100m);

        var result = wallet.ToString();

        result.Should().Contain("My Wallet");
        result.Should().Contain("100");
    }

    [Fact]
    public void Equality_WithSameId_ShouldBeEqual()
    {
        var (wallet1, wallet2) = EntityTestHelper.CreateTwoWithSameId(() =>
            UserWallet.From(
                "user123".ToRequiredString(),
                "Test".ToRequiredString(),
                new Money(100m, "USD").ToRequiredReference()));

        wallet1.Should().Be(wallet2);
        (wallet1 == wallet2).Should().BeTrue();
        wallet1.GetHashCode().Should().Be(wallet2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentId_ShouldNotBeEqual()
    {
        var (wallet1, wallet2) = EntityTestHelper.CreateTwoWithDifferentIds(() =>
            UserWallet.From(
                "user123".ToRequiredString(),
                "Test".ToRequiredString(),
                new Money(100m, "USD").ToRequiredReference()));

        wallet1.Should().NotBe(wallet2);
        (wallet1 == wallet2).Should().BeFalse();
        (wallet1 != wallet2).Should().BeTrue();
    }

    [Fact]
    public void Equality_WithSameIdButDifferentData_ShouldBeEqual()
    {
        var wallet1 = UserWallet.From(
            "user123".ToRequiredString(),
            "Savings".ToRequiredString(),
            new Money(100m, "USD").ToRequiredReference());

        var wallet2 = UserWallet.From(
            "user999".ToRequiredString(),
            "Checking".ToRequiredString(),
            new Money(500m, "EUR").ToRequiredReference());

        var sharedId = Guid.NewGuid();
        EntityTestHelper.SetId(wallet1, sharedId);
        EntityTestHelper.SetId(wallet2, sharedId);

        wallet1.Should().Be(wallet2);
    }

    private UserWallet CreateTestWallet(string name = "Test Wallet", decimal balance = 100m, string? currency = null)
    {
        return UserWallet.From(
            "user123".ToRequiredString(),
            name.ToRequiredString(),
            new Money(balance, currency ?? "USD").ToRequiredReference());
    }
}
