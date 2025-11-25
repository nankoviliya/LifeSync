using FluentAssertions;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Login.Models;
using LifeSync.API.Features.Authentication.Register.Models;
using LifeSync.Tests.Integration.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LifeSync.Tests.Integration.Features.Authentication;

public class LoginEndpointTests : IntegrationTestsBase
{
    public LoginEndpointTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_ReturnsValidTokenResponse_WhenRequestIsSuccessful()
    {
        RegisterRequest registerUserRequest = DefaultUserAccount.RegisterUserRequest;

        LoginRequest loginRequest = new()
        {
            Email = registerUserRequest.Email, Password = registerUserRequest.Password
        };

        HttpResponseMessage response = await HttpClient.PostAsJsonAsync($"/api/auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        TokenResponse? responseData = await response.Content.ReadFromJsonAsync<TokenResponse>();

        responseData.Should().NotBeNull();

        responseData.Token.Should().NotBeNullOrEmpty();
        responseData.Expiry.Should().BeAfter(DateTime.UtcNow);
        responseData.Expiry.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), TimeSpan.FromMinutes(1));

        // Verify token allows access to authenticated endpoints
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", responseData.Token);
        HttpResponseMessage
            authenticatedResponse = await HttpClient.GetAsync("/api/account"); // Should be 100% with auth

        authenticatedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}