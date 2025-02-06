using System.Text.Json.Serialization;

namespace LifeSync.API.Secrets.Models;

public class DbConnectionSecrets
{
    [JsonPropertyName("Username")]
    public string Username { get; set; }

    [JsonPropertyName("Password")]
    public string Password { get; set; }

    [JsonPropertyName("Engine")]
    public string Engine { get; set; }

    [JsonPropertyName("Host")]
    public string Host { get; set; }

    [JsonPropertyName("Port")]
    public int Port { get; set; }

    [JsonPropertyName("DbInstanceIdentifier")]
    public string DbInstanceIdentifier { get; set; }
}