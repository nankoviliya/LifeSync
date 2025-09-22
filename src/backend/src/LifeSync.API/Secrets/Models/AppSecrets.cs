using System.Text.Json.Serialization;

namespace LifeSync.API.Secrets.Models;

/// <summary>
/// Application secrets configuration loaded from environment variables or configuration files.
/// </summary>
public sealed class AppSecrets
{
    [JsonPropertyName("DOCKER")]
    public required bool IsDocker { get; init; }
    
    [JsonPropertyName("DB_USER")]
    public required string DbUser { get; init; }

    [JsonPropertyName("DB_PASSWD")]
    public required string DbPasswd { get; init; }

    [JsonPropertyName("DB_HOST")]
    public required string DbHost { get; init; }

    [JsonPropertyName("DB_PORT")]
    public required int DbPort { get; init; }

    [JsonPropertyName("DB_NAME")]
    public required string DbName { get; init; }
    
    [JsonPropertyName("JWT_SECRET_KEY")]
    public required string JwtSecretKey { get; init; }

    [JsonPropertyName("JWT_ISSUER")]
    public required string JwtIssuer { get; init; }

    [JsonPropertyName("JWT_AUDIENCE")]
    public required string JwtAudience { get; init; }

    [JsonPropertyName("JWT_EXPIRY_MINUTES")]
    public required int JwtExpiryMinutes { get; init; }

    [JsonIgnore]
    public DbConnectionSecrets Database =>
        DbConnectionSecrets.Create(DbUser, DbPasswd, DbHost, DbPort, DbName);

    [JsonIgnore]
    public JwtSecrets JWT => JwtSecrets.Create(JwtSecretKey, JwtIssuer, JwtAudience, JwtExpiryMinutes);
}