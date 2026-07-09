using Base.Contracts.Keycloak;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Keycloak.AdminApi;

/// <summary>
/// Provides dependency injection helpers for the Keycloak admin API client.
/// </summary>
public static class KeycloakAdminExtensions
{
    /// <summary>
    /// Registers <see cref="IKeycloakAdminClient"/> (implemented by <see cref="KeycloakAdminClient"/>) as a typed
    /// <see cref="HttpClient"/> configured for Keycloak.
    /// </summary>
    /// <param name="services">The service collection to register the client with.</param>
    /// <param name="options">The Keycloak admin API settings.</param>
    /// <returns>The HTTP client builder created by the registration so it can be further configured.</returns>
    public static IHttpClientBuilder AddKeycloakAdminClient(this IServiceCollection services, KeycloakAdminOptions options)
    {
        services.AddSingleton(options);

        return services.AddHttpClient<IKeycloakAdminClient, KeycloakAdminClient>(client =>
        {
            client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
        });
    }
}
