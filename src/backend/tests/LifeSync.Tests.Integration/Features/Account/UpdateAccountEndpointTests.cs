using FluentAssertions;
using LifeSync.API.Features.Account.Shared;
using LifeSync.API.Features.Account.UpdateAccount.Models;
using LifeSync.API.Features.Authentication.Login.Models;
using LifeSync.API.Features.Authentication.Register.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.Tests.Integration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LifeSync.Tests.Integration.Features.Account;

public class UpdateAccountEndpointTests : IntegrationTestsBase
{
    public UpdateAccountEndpointTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Update_ShouldUpdateAccount_WhenRequestIsValid()
    {
        RegisterRequest registerUserRequest = DefaultUserAccount.RegisterUserRequest;

        User? userBeforeUpdate = await DbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == registerUserRequest.Email);

        LoginResponse loginResponse = await LoginUserAsync(registerUserRequest.Email, registerUserRequest.Password);

        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);

        UpdateAccountRequest updateAccountRequest = new()
        {
            FirstName = "Updated FirstName",
            LastName = "Updated LastName",
            LanguageId = "A0B10003-1A2B-4C3D-9E10-000000000003"
        };

        HttpResponseMessage updateAccountResponse =
            await HttpClient.PutAsJsonAsync($"/api/account", updateAccountRequest);

        updateAccountResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        string responseString = await updateAccountResponse.Content.ReadAsStringAsync();

        responseString.Should().NotBeNullOrEmpty();
        responseString.Should().Contain(AccountResultMessages.UserProfileUpdated);

        User? updatedUser = await DbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == registerUserRequest.Email);

        // Check if target properties were updated
        updatedUser.FirstName.Should().Be(updateAccountRequest.FirstName);
        updatedUser.LastName.Should().Be(updateAccountRequest.LastName);
        updatedUser.LanguageId.Should().Be(updateAccountRequest.LanguageId);

        // Check if all remaining properties were unchanged
        updatedUser.Id.Should().Be(userBeforeUpdate.Id);
        updatedUser.Email.Should().Be(userBeforeUpdate.Email);
        updatedUser.Balance.Should().Be(userBeforeUpdate.Balance);
        updatedUser.CurrencyPreference.Should().Be(userBeforeUpdate.CurrencyPreference);
    }
}