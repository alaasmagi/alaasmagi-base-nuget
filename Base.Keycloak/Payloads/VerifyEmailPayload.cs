using System.Text.Json.Serialization;

namespace Base.Keycloak.Payloads;

public class VerifyEmailPayload
{
    [JsonPropertyName("userId")]
    public required string UserId { get; init; }

    [JsonPropertyName("email")]
    public required string Email { get; init; }

    [JsonPropertyName("verifyLink")]
    public required string VerifyLink { get; init; }

    [JsonPropertyName("expiresAt")]
    public required string ExpiresAt { get; init; }

    [JsonPropertyName("expiresInMinutes")]
    public int ExpiresInMinutes { get; init; }

    [JsonPropertyName("locale")]
    public string? Locale { get; init; }
}