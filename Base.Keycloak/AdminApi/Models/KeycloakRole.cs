using System.Text.Json.Serialization;

namespace Base.Keycloak.AdminApi.Models;

public class KeycloakRole
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }
}