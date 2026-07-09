namespace Base.Contracts.Keycloak;

/// <summary>
/// Defines a client contract for common Keycloak admin user operations.
/// </summary>
public interface IKeycloakAdminClient
{
    /// <summary>
    /// Creates a Keycloak user and returns the created user identifier when Keycloak includes it in the location header.
    /// </summary>
    /// <param name="user">The user payload to create.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that resolves to the created user identifier, or <see langword="null"/> when it cannot be read.</returns>
    Task<string?> CreateUserAsync(IKeycloakUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a Keycloak user by identifier.
    /// </summary>
    /// <param name="userId">The Keycloak user identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a Keycloak user by identifier.
    /// </summary>
    /// <param name="userId">The Keycloak user identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that resolves to the matching user, or <see langword="null"/> when Keycloak does not return a successful response.</returns>
    Task<IKeycloakUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
}

