namespace Base.Keycloak.Authentication;

/// <summary>
/// Provides Keycloak authentication settings for JWT bearer and OpenID Connect registration.
/// </summary>
public class KeycloakOptions
{
    /// <summary>
    /// Gets the Keycloak authority URL.
    /// </summary>
    public required string Authority { get; init; }

    /// <summary>
    /// Gets the expected JWT audience.
    /// </summary>
    public string? Audience { get; init; }

    /// <summary>
    /// Gets the Keycloak client identifier.
    /// </summary>
    public string? ClientId { get; init; }

    /// <summary>
    /// Gets the Keycloak client secret used by OpenID Connect flows.
    /// </summary>
    public string? ClientSecret { get; init; }

    /// <summary>
    /// Gets a value indicating whether HTTPS metadata is required.
    /// </summary>
    public bool RequireHttpsMetadata { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether client roles should be added as role claims.
    /// </summary>
    public bool IncludeClientRoles { get; init; } = true;
}
