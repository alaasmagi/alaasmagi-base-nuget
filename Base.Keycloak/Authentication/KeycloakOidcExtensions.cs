namespace Base.Keycloak.Authentication;

using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides dependency injection helpers for Keycloak OpenID Connect authentication.
/// </summary>
public static class KeycloakOidcExtensions
{
    /// <summary>
    /// Registers cookie and OpenID Connect authentication configured for Keycloak.
    /// </summary>
    /// <param name="services">The service collection to register authentication with.</param>
    /// <param name="options">The Keycloak authentication settings.</param>
    /// <returns>The authentication builder created by the registration.</returns>
    public static AuthenticationBuilder AddKeycloakOidc(this IServiceCollection services, KeycloakOptions options)
    {
        services.AddSingleton(options);
        services.AddTransient<IClaimsTransformation, KeycloakRoleClaimsTransformation>();
        services.AddOpenIdConnectAccessTokenManagement();

        return services.AddAuthentication(auth =>
            {
                auth.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(oidc =>
            {
                oidc.Authority = options.Authority;
                oidc.ClientId = options.ClientId;
                oidc.ClientSecret = options.ClientSecret;
                oidc.ResponseType = "code";
                oidc.SaveTokens = true;
                oidc.Scope.Add("offline_access");
                oidc.GetClaimsFromUserInfoEndpoint = true;
                oidc.TokenValidationParameters.NameClaimType = "preferred_username";
            });
    }
}
