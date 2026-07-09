using System.Text.Json.Serialization;

namespace Base.Keycloak.Events;

/// <summary>
/// Represents the <c>content</c> payload of a Keycloak user lifecycle event
/// (<c>user.created</c>, <c>user.updated</c>, <c>user.enabled</c>, <c>user.disabled</c>, <c>user.deleted</c>).
/// Only <see cref="UserId"/> is guaranteed to be present; the remaining fields are omitted for
/// events such as <c>user.deleted</c>.
/// </summary>
public class UserEventContent
{
    /// <summary>
    /// Gets the Keycloak user identifier.
    /// </summary>
    [JsonPropertyName("userId")]
    public required string UserId { get; init; }

    /// <summary>
    /// Gets the user's email address, when supplied.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    /// <summary>
    /// Gets the user's user name, when supplied.
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; init; }

    /// <summary>
    /// Gets the user's full name, when supplied.
    /// </summary>
    [JsonPropertyName("fullName")]
    public string? FullName { get; init; }

    /// <summary>
    /// Gets the user's locale, when supplied.
    /// </summary>
    [JsonPropertyName("locale")]
    public string? Locale { get; init; }
}

