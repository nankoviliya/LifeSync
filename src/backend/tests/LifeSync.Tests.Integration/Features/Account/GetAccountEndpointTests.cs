using FluentAssertions;
using LifeSync.API.Features.Account.GetAccount.Models;
using LifeSync.API.Features.Authentication.Login.Models;
using LifeSync.API.Features.Authentication.Register.Models;
using LifeSync.API.Shared;
using LifeSync.Tests.Integration.Infrastructure;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LifeSync.Tests.Integration.Features.Account;

public class GetAccountEndpointTests : IntegrationTestsBase
{
    public GetAccountEndpointTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Get_ReturnsUserAccount_WhenRequestIsValid()
    {
        RegisterRequest registerUserRequest = DefaultUserAccount.RegisterUserRequest;

        LoginResponse loginResponse = await LoginUserAsync(registerUserRequest.Email, registerUserRequest.Password);

        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
        HttpResponseMessage getAccountResponse = await HttpClient.GetAsync($"/api/account");

        GetAccountResponse? accountData = await getAccountResponse.Content.ReadFromJsonAsync<GetAccountResponse>();

        accountData.Should().NotBeNull();
        accountData.UserId.Should().NotBeNullOrEmpty();
        accountData.UserName.Should().NotBeNullOrEmpty();
        accountData.LastName.Should().NotBeNullOrEmpty();
        accountData.BalanceAmount.Should().BeGreaterThanOrEqualTo(0);
        CurrencyRegistry.IsSupported(accountData.BalanceCurrency).Should().BeTrue();
        accountData.Language.Should().NotBeNull();
        accountData.Language.Id.Should().NotBeEmpty();
        accountData.Language.Name.Should().NotBeNullOrEmpty();
        accountData.Language.Code.Should().NotBeNullOrEmpty();
    }
}