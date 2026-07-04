using System.Net.Http.Json;
using Base.Keycloak.AdminApi.Models;

namespace Base.Keycloak.AdminApi;

public class KeycloakAdminClient
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakAdminOptions _options;
    private string? _cachedToken;
    private DateTimeOffset _tokenExpiresAt;

    public KeycloakAdminClient(HttpClient httpClient, KeycloakAdminOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<string?> CreateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default)
    {
        await EnsureAuthenticatedAsync(cancellationToken);

        var response = await _httpClient.PostAsJsonAsync(
            $"/admin/realms/{_options.Realm}/users", user, cancellationToken);
        response.EnsureSuccessStatusCode();

        var location = response.Headers.Location?.ToString();
        return location?.Split('/').LastOrDefault();
    }

    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        await EnsureAuthenticatedAsync(cancellationToken);

        var response = await _httpClient.DeleteAsync(
            $"/admin/realms/{_options.Realm}/users/{userId}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }

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