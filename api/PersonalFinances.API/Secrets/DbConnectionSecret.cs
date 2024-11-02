using System.Text.Json.Serialization;

namespace PersonalFinances.API.Secrets;

public class DbConnectionSecret
{
    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("engine")]
    public string Engine { get; set; }

    [JsonPropertyName("host")]
    public string Host { get; set; }

    [JsonPropertyName("port")]
    public int Port { get; set; }

    [JsonPropertyName("dbInstanceIdentifier")]
    public string DbInstanceIdentifier { get; set; }
}