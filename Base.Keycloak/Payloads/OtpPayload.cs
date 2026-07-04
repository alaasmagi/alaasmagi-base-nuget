using System.Text.Json.Serialization;

namespace Base.Keycloak.Payloads;

/// <summary>
/// Represents the payload for a one-time password email action.
/// </summary>
public class OtpPayload
{
    /// <summary>
    /// Gets the Keycloak user identifier.
    /// </summary>
    [JsonPropertyName("userId")]
    public required string UserId { get; init; }

    /// <summary>
    /// Gets the email address that should receive the one-time password.
    /// </summary>
    [JsonPropertyName("email")]
    public required string Email { get; init; }

    /// <summary>
    /// Gets the one-time password code.
    /// </summary>
    [JsonPropertyName("otpCode")]
    public required string OtpCode { get; init; }

    /// <summary>
    /// Gets the expiration timestamp for the one-time password.
    /// </summary>
    [JsonPropertyName("expiresAt")]
    public required string ExpiresAt { get; init; }

    /// <summary>
    /// Gets the number of minutes until the one-time password expires.
    /// </summary>
    [JsonPropertyName("expiresInMinutes")]
    public int ExpiresInMinutes { get; init; }

    /// <summary>
    /// Gets the optional locale for the email action.
    /// </summary>
    [JsonPropertyName("locale")]
    public string? Locale { get; init; }
}
