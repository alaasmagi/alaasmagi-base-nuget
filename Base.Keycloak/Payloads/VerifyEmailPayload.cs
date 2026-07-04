using System.Text.Json.Serialization;

namespace Base.Keycloak.Payloads;

/// <summary>
/// Represents the payload for a verify-email action.
/// </summary>
public class VerifyEmailPayload
{
    /// <summary>
    /// Gets the Keycloak user identifier.
    /// </summary>
    [JsonPropertyName("userId")]
    public required string UserId { get; init; }

    /// <summary>
    /// Gets the email address that should receive the verification link.
    /// </summary>
    [JsonPropertyName("email")]
    public required string Email { get; init; }

    /// <summary>
    /// Gets the email verification link.
    /// </summary>
    [JsonPropertyName("verifyLink")]
    public required string VerifyLink { get; init; }

    /// <summary>
    /// Gets the expiration timestamp for the verification link.
    /// </summary>
    [JsonPropertyName("expiresAt")]
    public required string ExpiresAt { get; init; }

    /// <summary>
    /// Gets the number of minutes until the verification link expires.
    /// </summary>
    [JsonPropertyName("expiresInMinutes")]
    public int ExpiresInMinutes { get; init; }

    /// <summary>
    /// Gets the optional locale for the email action.
    /// </summary>
    [JsonPropertyName("locale")]
    public string? Locale { get; init; }
}
