using FluentAssertions;
using LifeSync.API.Features.AccountImport;
using LifeSync.API.Features.AccountImport.DataReaders;
using LifeSync.API.Features.AccountImport.Models;
using LifeSync.Common.Required;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace LifeSync.Tests.Unit.Features.AccountImport.DataReaders;

public class JsonAccountDataReaderTests
{
    [Fact]
    public void Format_ShouldReturnJson()
    {
        JsonAccountDataReader dataReader = new();

        AccountImportFileFormat result = dataReader.Format;

        result.Should().Be(AccountImportFileFormat.Json);
    }

    [Fact]
    public async Task ReadAsync_WithValidJson_ShouldDeserializeSuccessfully()
    {
        JsonAccountDataReader dataReader = new();

        ImportAccountData expectedData = ImportData.GetData(Guid.NewGuid().ToRequiredStruct());

        string json = JsonSerializer.Serialize(expectedData);
        IFormFile file = ImportData.CreateSubstituteFormFile("test.json", json);

        ImportAccountData? result = await dataReader.ReadAsync(file, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().NotBeNull();
        result!.ProfileData.BalanceAmount.Should().Be(expectedData.ProfileData.BalanceAmount);
        result.ProfileData.BalanceCurrency.Should().Be(expectedData.ProfileData.BalanceCurrency);
        result.ExpenseTransactions.Should().HaveCount(1);
        result.IncomeTransactions.Should().HaveCount(1);
    }

    [Fact]
    public async Task ReadAsync_WithEmptyJson_ShouldReturnNull()
    {
        JsonAccountDataReader dataReader = new();

        IFormFile file = ImportData.CreateSubstituteFormFile("empty.json", "null");

        ImportAccountData? result = await dataReader.ReadAsync(file, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ReadAsync_WithInvalidJson_ShouldThrowJsonException()
    {
        JsonAccountDataReader dataReader = new();

        IFormFile file = ImportData.CreateSubstituteFormFile("invalid.json", "{ invalid json }");

        await Assert.ThrowsAsync<JsonException>(() =>
            dataReader.ReadAsync(file, CancellationToken.None));
    }

    [Fact]
    public async Task ReadAsync_WithEmptyFile_ShouldThrowJsonException()
    {
        JsonAccountDataReader dataReader = new();

        IFormFile file = ImportData.CreateSubstituteFormFile("empty.json", string.Empty);

        await Assert.ThrowsAsync<JsonException>(() =>
            dataReader.ReadAsync(file, CancellationToken.None));
    }
}