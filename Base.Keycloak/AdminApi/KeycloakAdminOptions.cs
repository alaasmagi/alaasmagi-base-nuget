namespace Base.Keycloak.AdminApi;

/// <summary>
/// Provides Keycloak admin API connection settings.
/// </summary>
public class KeycloakAdminOptions
{
    /// <summary>
    /// Gets the base URL of the Keycloak server.
    /// </summary>
    public required string BaseUrl { get; init; }

    /// <summary>
    /// Gets the Keycloak realm used for admin API calls.
    /// </summary>
    public required string Realm { get; init; }

    /// <summary>
    /// Gets the client identifier used for client credentials authentication.
    /// </summary>
    public required string ClientId { get; init; }

    /// <summary>
    /// Gets the client secret used for client credentials authentication.
    /// </summary>
    public required string ClientSecret { get; init; }
}
