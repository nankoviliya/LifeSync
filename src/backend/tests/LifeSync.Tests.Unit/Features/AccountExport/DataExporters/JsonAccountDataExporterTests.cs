using FluentAssertions;
using LifeSync.API.Features.AccountExport;
using LifeSync.API.Features.AccountExport.DataExporters;
using LifeSync.API.Features.AccountExport.Models;
using System.Text;
using System.Text.Json;

namespace LifeSync.UnitTests.Features.AccountExport.DataExporters;

public class JsonAccountDataExporterTests
{
    [Fact]
    public void Format_ShouldReturnJson()
    {
        JsonAccountDataExporter exporter = new();

        ExportAccountFileFormat result = exporter.Format;

        result.Should().Be(ExportAccountFileFormat.Json);
    }

    [Fact]
    public async Task Export_WithValidData_ShouldSerializeSuccessfully()
    {
        JsonAccountDataExporter exporter = new();

        ExportAccountData accountData = ExportData.GetData();

        ExportAccountResponse result = await exporter.Export(accountData, CancellationToken.None);

        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
        result.ContentType.Should().Be("application/json");
        result.FileName.Should().Be("account-data.json");
    }

    [Fact]
    public async Task Export_ShouldProduceValidJson()
    {
        JsonAccountDataExporter exporter = new();

        ExportAccountData accountData = ExportData.GetData();

        ExportAccountResponse result = await exporter.Export(accountData, CancellationToken.None);

        string json = Encoding.UTF8.GetString(result.Data);
        ExportAccountData? deserializedData = JsonSerializer.Deserialize<ExportAccountData>(json);

        deserializedData.Should().NotBeNull();
        deserializedData.Should().BeEquivalentTo(accountData);
    }

    [Fact]
    public async Task Export_WithEmptyData_ShouldSerializeSuccessfully()
    {
        JsonAccountDataExporter exporter = new();

        ExportAccountData accountData = new();

        ExportAccountResponse result = await exporter.Export(accountData, CancellationToken.None);

        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
        result.ContentType.Should().Be("application/json");
    }
}