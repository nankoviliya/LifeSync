using FluentAssertions;
using LifeSync.API.Features.AccountExport;
using LifeSync.API.Features.AccountExport.Models;
using LifeSync.API.Features.Authentication.Login.Models;
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
        LoginResponse loginResponse = await LoginUserAsync(registerRequest.Email, registerRequest.Password);

        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);

        ExportAccountRequest exportRequest = new() { Format = ExportAccountFileFormat.Json };

        HttpResponseMessage exportResponse = await HttpClient.PostAsJsonAsync("/api/accountExport", exportRequest);
        ExportAccountResponse? data = await exportResponse.Content.ReadFromJsonAsync<ExportAccountResponse>();

        // Response shape
        exportResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        data.Should().NotBeNull();
        data!.ContentType.Should().Be("application/json");
        data.FileName.Should().MatchRegex(@".+\.json$");
        data.EncodedData.Should().NotBeNullOrEmpty();

        // Decodable
        ExportAccountData exportedData = DecodeExportData(data.EncodedData);
        exportedData.Should().NotBeNull();

        // Profile matches registered user
        exportedData!.ProfileData.Should().NotBeNull();
        exportedData.ProfileData.UserId.Should().NotBeNullOrEmpty();
        exportedData.ProfileData.Email.Should().Be(registerRequest.Email);
        exportedData.ProfileData.FirstName.Should().Be(registerRequest.FirstName);
        exportedData.ProfileData.LastName.Should().Be(registerRequest.LastName);
        exportedData.ProfileData.LanguageCode.Should().NotBeNullOrEmpty();
        exportedData.ProfileData.BalanceCurrency.Should().NotBeNullOrEmpty();
        exportedData.ProfileData.BalanceAmount.Should().BeGreaterThanOrEqualTo(0);

        // Transactions present and well-formed
        exportedData.ExpenseTransactions.Should().NotBeNull();
        exportedData.IncomeTransactions.Should().NotBeNull();

        foreach (ExportAccountExpenseTransaction tx in exportedData.ExpenseTransactions)
        {
            tx.Id.Should().NotBeEmpty();
            tx.Amount.Should().BeGreaterThan(0);
            tx.Currency.Should().NotBeNullOrEmpty();
            tx.Date.Should().BeBefore(DateTime.UtcNow);
        }

        foreach (ExportAccountIncomeTransaction tx in exportedData.IncomeTransactions)
        {
            tx.Id.Should().NotBeEmpty();
            tx.Amount.Should().BeGreaterThan(0);
            tx.Currency.Should().NotBeNullOrEmpty();
            tx.Date.Should().BeBefore(DateTime.UtcNow);
        }

        // No sensitive data exposed
        string encodedDataJson = Encoding.UTF8.GetString(Convert.FromBase64String(data.EncodedData));
        encodedDataJson.Should().NotContain("password");
        encodedDataJson.Should().NotContain("passwordHash");
    }

    private static ExportAccountData DecodeExportData(string encodedData)
    {
        byte[] bytes = Convert.FromBase64String(encodedData);
        string json = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<ExportAccountData>(json)!;
    }
}