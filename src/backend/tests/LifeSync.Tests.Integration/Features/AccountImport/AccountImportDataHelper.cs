using LifeSync.API.Features.AccountImport;
using System.Net.Http.Headers;
using System.Text;

namespace LifeSync.Tests.Integration.Features.AccountImport;

public static class AccountImportDataHelper
{
    // Overload for string content (JSON, CSV, XML, etc.)
    public static MultipartFormDataContent CreateImportForm(
        AccountImportFileFormat format,
        string content,
        string fileName,
        string contentType)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        return CreateImportForm(format, bytes, fileName, contentType);
    }

    // Base method for byte array content (handles all formats)
    public static MultipartFormDataContent CreateImportForm(
        AccountImportFileFormat format,
        byte[] fileBytes,
        string fileName,
        string contentType)
    {
        MultipartFormDataContent form = new();

        ByteArrayContent fileContent = new(fileBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        form.Add(fileContent, "File", fileName);

        form.Add(new StringContent(((int)format).ToString()), "Format");

        return form;
    }
}