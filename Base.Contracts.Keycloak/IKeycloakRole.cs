namespace Base.Contracts.Keycloak;

/// <summary>
/// Defines a Keycloak role contract returned by or sent to the admin API.
/// </summary>
public interface IKeycloakRole
{
    /// <summary>
    /// Gets the Keycloak role identifier.
    /// </summary>
    public string? Id { get; }

    /// <summary>
    /// Gets the Keycloak role name.
    /// </summary>
    public string Name { get; }
}

