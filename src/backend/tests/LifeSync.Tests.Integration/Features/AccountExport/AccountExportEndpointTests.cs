using FluentAssertions;
using LifeSync.API.Features.AccountExport;
using LifeSync.API.Features.AccountExport.Models;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Register.Models;
using LifeSync.Tests.Integration.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace LifeSync.Tests.Integration.Features.AccountExport;

public class AccountExportEndpointTests : IntegrationTestsBase
{
    public AccountExportEndpointTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Export_ShouldReturnValidJsonFile_WhenJsonImportIsSelected()
    {
        RegisterRequest registerRequest = DefaultUserAccount.RegisterUserRequest;
        TokenResponse tokenResponse = await LoginUserAsync(registerRequest.Email, registerRequest.Password);

        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

        ExportAccountRequest exportRequest = new() { Format = ExportAccountFileFormat.Json };

        HttpResponseMessage exportResponse = await HttpClient.PostAsJsonAsync("/api/accountExport", exportRequest);
        ExportAccountResponse? data = await exportResponse.Content.ReadFromJsonAsync<ExportAccountResponse>();

        exportResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        data.Should().NotBeNull();
        data!.ContentType.Should().Be("application/json");
        data.FileName.Should().EndWith(".json");
        data.EncodedData.Should().NotBeNullOrEmpty();

        ExportAccountData exportedData = DecodeExportData(data.EncodedData);
        exportedData.Should().NotBeNull();

        // Profile data matches user
        exportedData!.ProfileData.Should().NotBeNull();
        exportedData.ProfileData.Email.Should().Be(registerRequest.Email);
        exportedData.ProfileData.FirstName.Should().Be(registerRequest.FirstName);
        exportedData.ProfileData.LastName.Should().Be(registerRequest.LastName);

        // Transactions are present
        exportedData.ExpenseTransactions.Should().NotBeNull();
        exportedData.IncomeTransactions.Should().NotBeNull();

        // No sensitive data exposed
        string encodedDataJson = Encoding.UTF8.GetString(Convert.FromBase64String(data.EncodedData));
        encodedDataJson.Should().NotContain("password", "passwordHash");
    }

    private static ExportAccountData DecodeExportData(string encodedData)
    {
        byte[] bytes = Convert.FromBase64String(encodedData);
        string json = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<ExportAccountData>(json)!;
    }
}