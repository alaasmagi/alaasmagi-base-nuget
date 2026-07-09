using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Keycloak.Authentication;

/// <summary>
/// Provides dependency injection helpers for Keycloak JWT bearer authentication.
/// </summary>
public static class KeycloakJwtExtensions
{
    /// <summary>
    /// Registers JWT bearer authentication configured for Keycloak.
    /// </summary>
    /// <param name="services">The service collection to register authentication with.</param>
    /// <param name="options">The Keycloak authentication settings.</param>
    /// <returns>The authentication builder created by the registration.</returns>
    public static AuthenticationBuilder AddKeycloakJwtBearer(this IServiceCollection services, KeycloakOptions options)
    {
        services.AddSingleton(options);
        services.AddTransient<IClaimsTransformation, KeycloakRoleClaimsTransformation>();

        return services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.Authority = options.Authority;
                jwtOptions.Audience = options.Audience;
                jwtOptions.RequireHttpsMetadata = options.RequireHttpsMetadata;
            });
    }
}
