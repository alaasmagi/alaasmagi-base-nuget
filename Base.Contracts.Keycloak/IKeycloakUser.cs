namespace Base.Contracts.Keycloak;

/// <summary>
/// Defines a Keycloak user contract returned by or sent to the admin API.
/// </summary>
public interface IKeycloakUser
{
    /// <summary>
    /// Gets the Keycloak user identifier.
    /// </summary>
    public string? Id { get; }

    /// <summary>
    /// Gets the Keycloak user name.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the user's email address.
    /// </summary>
    public string? Email { get; }

    /// <summary>
    /// Gets a value indicating whether the user is enabled.
    /// </summary>
    public bool Enabled { get; }

    /// <summary>
    /// Gets a value indicating whether the user's email address has been verified.
    /// </summary>
    public bool EmailVerified { get; }
}

