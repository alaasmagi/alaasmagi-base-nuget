using System.Text.Json.Serialization;

namespace Base.Keycloak.Payloads;

/// <summary>
/// Represents the payload for a password reset email action.
/// </summary>
public class PasswordResetPayload
{
    /// <summary>
    /// Gets the Keycloak user identifier.
    /// </summary>
    [JsonPropertyName("userId")]
    public required string UserId { get; init; }

    /// <summary>
    /// Gets the email address that should receive the password reset link.
    /// </summary>
    [JsonPropertyName("email")]
    public required string Email { get; init; }

    /// <summary>
    /// Gets the password reset link.
    /// </summary>
    [JsonPropertyName("resetLink")]
    public required string ResetLink { get; init; }

    /// <summary>
    /// Gets the expiration timestamp for the password reset link.
    /// </summary>
    [JsonPropertyName("expiresAt")]
    public required string ExpiresAt { get; init; }

    /// <summary>
    /// Gets the number of minutes until the password reset link expires.
    /// </summary>
    [JsonPropertyName("expiresInMinutes")]
    public int ExpiresInMinutes { get; init; }

    /// <summary>
    /// Gets the optional locale for the email action.
    /// </summary>
    [JsonPropertyName("locale")]
    public string? Locale { get; init; }
}
