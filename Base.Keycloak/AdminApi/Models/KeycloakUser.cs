namespace Base.Keycloak.AdminApi.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a Keycloak user payload returned by or sent to the admin API.
/// </summary>
public class KeycloakUser
{
    /// <summary>
    /// Gets the Keycloak user identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Gets the Keycloak user name.
    /// </summary>
    [JsonPropertyName("username")]
    public required string Username { get; init; }

    /// <summary>
    /// Gets the user's email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    /// <summary>
    /// Gets a value indicating whether the user is enabled.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the user's email address has been verified.
    /// </summary>
    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; init; }
}
