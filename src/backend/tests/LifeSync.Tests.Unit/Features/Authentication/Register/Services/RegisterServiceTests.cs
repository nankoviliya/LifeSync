using FluentAssertions;
using LifeSync.API.Features.Authentication.Register.Models;
using LifeSync.API.Features.Authentication.Register.Services;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.Common.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LifeSync.UnitTests.Features.Authentication.Register.Services;

public class RegisterServiceTests
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<RegisterService> _logger;
    private readonly RegisterService _sut;

    public RegisterServiceTests()
    {
        _userManager = Substitute.For<UserManager<User>>(
            Substitute.For<IUserStore<User>>(),
            null, null, null, null, null, null, null, null);

        _logger = Substitute.For<ILogger<RegisterService>>();

        _sut = new RegisterService(_userManager, _logger);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenRegistrationIsSuccessful()
    {
        RegisterRequest request = new()
        {
            Email = "newuser@example.com",
            Password = "SecurePassword123!",
            FirstName = "John",
            LastName = "Doe",
            Balance = 1000m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        _userManager.CreateAsync(Arg.Any<User>(), request.Password)
            .Returns(Task.FromResult(IdentityResult.Success));

        MessageResult result = await _sut.RegisterAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Successfully registered");
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnFailure_WhenUserCreationFails()
    {
        RegisterRequest request = new()
        {
            Email = "existing@example.com",
            Password = "Password123!",
            FirstName = "Jane",
            LastName = "Smith",
            Balance = 500m,
            Currency = "EUR",
            LanguageId = Guid.NewGuid()
        };

        IdentityError[] errors = new[]
        {
            new IdentityError { Code = "DuplicateEmail", Description = "Email is already taken." }
        };

        _userManager.CreateAsync(Arg.Any<User>(), request.Password)
            .Returns(Task.FromResult(IdentityResult.Failed(errors)));

        MessageResult result = await _sut.RegisterAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Email is already taken.");
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnAllErrors_WhenMultipleErrorsOccur()
    {
        RegisterRequest request = new()
        {
            Email = "invalid@example.com",
            Password = "weak",
            FirstName = "Test",
            LastName = "User",
            Balance = 0m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        IdentityError[] errors = new[]
        {
            new IdentityError { Code = "PasswordTooShort", Description = "Password must be at least 6 characters." },
            new IdentityError { Code = "PasswordRequiresNonAlphanumeric", Description = "Password must contain a special character." }
        };

        _userManager.CreateAsync(Arg.Any<User>(), request.Password)
            .Returns(Task.FromResult(IdentityResult.Failed(errors)));

        MessageResult result = await _sut.RegisterAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain("Password must be at least 6 characters.");
        result.Errors.Should().Contain("Password must contain a special character.");
    }

    [Fact]
    public async Task RegisterAsync_ShouldCallUserManagerCreateAsync_WithCorrectUserData()
    {
        RegisterRequest request = new()
        {
            Email = "test@example.com",
            Password = "SecurePassword123!",
            FirstName = "Alice",
            LastName = "Brown",
            Balance = 2500m,
            Currency = "BGN",
            LanguageId = Guid.Parse("BE9E8EEA-E3A4-475E-B364-08218FB3CC6C")
        };

        User capturedUser = null!;

        _userManager.CreateAsync(Arg.Any<User>(), request.Password)
            .Returns(callInfo =>
            {
                capturedUser = callInfo.ArgAt<User>(0);
                return Task.FromResult(IdentityResult.Success);
            });

        await _sut.RegisterAsync(request);

        await _userManager.Received(1).CreateAsync(Arg.Any<User>(), request.Password);

        capturedUser.Should().NotBeNull();
        capturedUser.Email.Should().Be(request.Email);
        capturedUser.UserName.Should().Be(request.Email);
        capturedUser.FirstName.Should().Be(request.FirstName);
        capturedUser.LastName.Should().Be(request.LastName);
        capturedUser.Balance.Amount.Should().Be(request.Balance);
        capturedUser.Balance.CurrencyCode.Should().Be(request.Currency);
        capturedUser.CurrencyPreference.Should().Be(request.Currency);
        capturedUser.LanguageId.Should().Be(request.LanguageId);
    }

    [Fact]
    public async Task RegisterAsync_ShouldLogWarning_WhenRegistrationFails()
    {
        RegisterRequest request = new()
        {
            Email = "fail@example.com",
            Password = "Password123!",
            FirstName = "Failed",
            LastName = "User",
            Balance = 100m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        IdentityError[] errors = new[]
        {
            new IdentityError { Code = "Error1", Description = "Error description 1" },
            new IdentityError { Code = "Error2", Description = "Error description 2" }
        };

        _userManager.CreateAsync(Arg.Any<User>(), request.Password)
            .Returns(Task.FromResult(IdentityResult.Failed(errors)));

        await _sut.RegisterAsync(request);

        _logger.Received(1).Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Registration failed")),
            null,
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUserWithCorrectBalance_WhenBalanceIsZero()
    {
        RegisterRequest request = new()
        {
            Email = "zerobalance@example.com",
            Password = "Password123!",
            FirstName = "Zero",
            LastName = "Balance",
            Balance = 0m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        User capturedUser = null!;

        _userManager.CreateAsync(Arg.Any<User>(), request.Password)
            .Returns(callInfo =>
            {
                capturedUser = callInfo.ArgAt<User>(0);
                return Task.FromResult(IdentityResult.Success);
            });

        await _sut.RegisterAsync(request);

        await _userManager.Received(1).CreateAsync(Arg.Any<User>(), request.Password);
        capturedUser.Balance.Amount.Should().Be(0m);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUserWithCorrectBalance_WhenBalanceIsPositive()
    {
        RegisterRequest request = new()
        {
            Email = "positivebalance@example.com",
            Password = "Password123!",
            FirstName = "Positive",
            LastName = "Balance",
            Balance = 5000.50m,
            Currency = "EUR",
            LanguageId = Guid.NewGuid()
        };

        User capturedUser = null!;

        _userManager.CreateAsync(Arg.Any<User>(), request.Password)
            .Returns(callInfo =>
            {
                capturedUser = callInfo.ArgAt<User>(0);
                return Task.FromResult(IdentityResult.Success);
            });

        await _sut.RegisterAsync(request);

        await _userManager.Received(1).CreateAsync(Arg.Any<User>(), request.Password);
        capturedUser.Balance.Amount.Should().Be(5000.50m);
    }

    [Fact]
    public async Task RegisterAsync_ShouldSetUserNameAsEmail()
    {
        RegisterRequest request = new()
        {
            Email = "username@example.com",
            Password = "Password123!",
            FirstName = "User",
            LastName = "Name",
            Balance = 1000m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        User capturedUser = null!;

        _userManager.CreateAsync(Arg.Any<User>(), request.Password)
            .Returns(callInfo =>
            {
                capturedUser = callInfo.ArgAt<User>(0);
                return Task.FromResult(IdentityResult.Success);
            });

        await _sut.RegisterAsync(request);

        await _userManager.Received(1).CreateAsync(Arg.Any<User>(), request.Password);
        capturedUser.UserName.Should().Be(request.Email);
    }

    [Fact]
    public async Task RegisterAsync_ShouldHandleDifferentCurrencies()
    {
        string[] currencies = { "USD", "EUR", "BGN", "UAH" };

        foreach (string currency in currencies)
        {
            User capturedUser = null!;

            RegisterRequest request = new()
            {
                Email = $"user_{currency}@example.com",
                Password = "Password123!",
                FirstName = "Currency",
                LastName = "Test",
                Balance = 1000m,
                Currency = currency,
                LanguageId = Guid.NewGuid()
            };

            _userManager.CreateAsync(Arg.Any<User>(), request.Password)
                .Returns(callInfo =>
                {
                    capturedUser = callInfo.ArgAt<User>(0);
                    return Task.FromResult(IdentityResult.Success);
                });

            MessageResult result = await _sut.RegisterAsync(request);

            result.IsSuccess.Should().BeTrue();
            await _userManager.Received().CreateAsync(Arg.Any<User>(), request.Password);
            capturedUser.CurrencyPreference.Should().Be(currency);
        }
    }
}
