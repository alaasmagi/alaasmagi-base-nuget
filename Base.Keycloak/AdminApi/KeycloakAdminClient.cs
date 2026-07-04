using System.Net.Http.Json;
using Base.Keycloak.AdminApi.Models;

namespace Base.Keycloak.AdminApi;

/// <summary>
/// Provides a small client for common Keycloak admin user operations.
/// </summary>
public class KeycloakAdminClient
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakAdminOptions _options;
    private string? _cachedToken;
    private DateTimeOffset _tokenExpiresAt;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakAdminClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to call Keycloak.</param>
    /// <param name="options">The Keycloak admin API settings.</param>
    public KeycloakAdminClient(HttpClient httpClient, KeycloakAdminOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    /// <summary>
    /// Creates a Keycloak user and returns the created user identifier when Keycloak includes it in the location header.
    /// </summary>
    /// <param name="user">The user payload to create.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that resolves to the created user identifier, or <see langword="null"/> when it cannot be read.</returns>
    public async Task<string?> CreateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default)
    {
        await EnsureAuthenticatedAsync(cancellationToken);

        var response = await _httpClient.PostAsJsonAsync(
            $"/admin/realms/{_options.Realm}/users", user, cancellationToken);
        response.EnsureSuccessStatusCode();

        var location = response.Headers.Location?.ToString();
        return location?.Split('/').LastOrDefault();
    }

    /// <summary>
    /// Deletes a Keycloak user by identifier.
    /// </summary>
    /// <param name="userId">The Keycloak user identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        await EnsureAuthenticatedAsync(cancellationToken);

        var response = await _httpClient.DeleteAsync(
            $"/admin/realms/{_options.Realm}/users/{userId}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Retrieves a Keycloak user by identifier.
    /// </summary>
    /// <param name="userId">The Keycloak user identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that resolves to the matching user, or <see langword="null"/> when Keycloak does not return a successful response.</returns>
    public async Task<KeycloakUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        await EnsureAuthenticatedAsync(cancellationToken);

        var response = await _httpClient.GetAsync(
            $"/admin/realms/{_options.Realm}/users/{userId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<KeycloakUser>(cancellationToken);
    }

    private async Task EnsureAuthenticatedAsync(CancellationToken cancellationToken)
    {
        if (_cachedToken != null && DateTimeOffset.UtcNow < _tokenExpiresAt)
        {
            return;
        }

        var tokenResponse = await _httpClient.PostAsync(
            $"/realms/{_options.Realm}/protocol/openid-connect/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret
            }),
            cancellationToken);

        tokenResponse.EnsureSuccessStatusCode();

        var tokenData = await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken);
        _cachedToken = tokenData!.AccessToken;
        _tokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenData.ExpiresIn - 30);

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cachedToken);
    }

    private class TokenResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("access_token")]
        public required string AccessToken { get; init; }

        [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }
    }
}
