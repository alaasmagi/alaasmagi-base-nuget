namespace Base.Keycloak.Authentication;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

public static class KeycloakJwtExtensions
{
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