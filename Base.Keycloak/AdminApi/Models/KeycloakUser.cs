namespace Base.Keycloak.AdminApi.Models;

using System.Text.Json.Serialization;

public class KeycloakUser
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("username")]
    public required string Username { get; init; }

    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; } = true;

    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; init; }
}