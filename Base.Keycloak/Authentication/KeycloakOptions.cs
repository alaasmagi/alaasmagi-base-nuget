namespace Base.Keycloak.Authentication;

public class KeycloakOptions
{
    public required string Authority { get; init; }
    public string? Audience { get; init; }
    public string? ClientId { get; init; }
    public string? ClientSecret { get; init; }
    public bool RequireHttpsMetadata { get; init; } = true;
    public bool IncludeClientRoles { get; init; } = true;
}