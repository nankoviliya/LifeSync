using LifeSync.API.Features.AccountDataExchange.Models;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;
using System.Text.Json;

namespace LifeSync.API.Features.AccountDataExchange.Services.ExportData.ConcreteExporterFactory.ConcreteExporters
{
    public class JsonAccountDataFileExporter : BaseService, IAccountDataFileConcreteExporter
    {
        public ExportAccountFileFormat SupportedFormat => ExportAccountFileFormat.Json;

        public async Task<DataResult<ExportAccountFileResultDto>> ExportAccountData(ExportAccountDataDto accountData)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            await using var memoryStream = new MemoryStream();

            await JsonSerializer.SerializeAsync(memoryStream, accountData, options);

            memoryStream.Position = 0;
            byte[] fileBytes = memoryStream.ToArray();

            var exportResult = new ExportAccountFileResultDto
            {
                Data = fileBytes,
                ContentType = "application/json",
                FileName = "account-data.json"
            };

            return Success(exportResult);
        }
    }
}
