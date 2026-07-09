using System.Text.Json.Serialization;
using Base.Contracts.Keycloak;

namespace Base.Keycloak.AdminApi.Models;

/// <summary>
/// Represents a Keycloak role payload returned by or sent to the admin API.
/// </summary>
public class KeycloakRole : IKeycloakRole
{
    /// <summary>
    /// Gets the Keycloak role identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Gets the Keycloak role name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }
}

