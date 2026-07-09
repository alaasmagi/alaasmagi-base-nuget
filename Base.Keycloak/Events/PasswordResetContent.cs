using System.Text.Json.Serialization;

namespace Base.Keycloak.Events;

/// <summary>
/// Represents the <c>content</c> payload of a Keycloak password reset event (<c>user.password.reset</c>).
/// </summary>
public class PasswordResetContent
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
    /// Gets the user's full name.
    /// </summary>
    [JsonPropertyName("fullName")]
    public required string FullName { get; init; }

    /// <summary>
    /// Gets the Keycloak action link the user follows to reset their password.
    /// </summary>
    [JsonPropertyName("actionLink")]
    public required string ActionLink { get; init; }

    /// <summary>
    /// Gets the expiration timestamp for the action link.
    /// </summary>
    [JsonPropertyName("expiresAt")]
    public required DateTimeOffset ExpiresAt { get; init; }

    /// <summary>
    /// Gets the number of minutes until the action link expires.
    /// </summary>
    [JsonPropertyName("expiresInMinutes")]
    public int ExpiresInMinutes { get; init; }

    /// <summary>
    /// Gets the optional locale for the email action.
    /// </summary>
    [JsonPropertyName("locale")]
    public string? Locale { get; init; }
}

