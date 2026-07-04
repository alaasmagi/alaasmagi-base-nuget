namespace Base.Keycloak.Authentication;

using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

public class KeycloakRoleClaimsTransformation : IClaimsTransformation
{
    private readonly KeycloakOptions _options;

    public KeycloakRoleClaimsTransformation(KeycloakOptions options)
    {
        _options = options;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity)
        {
            return Task.FromResult(principal);
        }

        var realmAccessClaim = principal.FindFirst("realm_access");
        if (realmAccessClaim != null)
        {
            AddRolesFromJson(identity, principal, realmAccessClaim.Value, "roles");
        }

        if (_options.IncludeClientRoles && !string.IsNullOrEmpty(_options.ClientId))
        {
            var resourceAccessClaim = principal.FindFirst("resource_access");
            if (resourceAccessClaim != null)
            {
                AddClientRolesFromJson(identity, principal, resourceAccessClaim.Value, _options.ClientId);
            }
        }

        return Task.FromResult(principal);
    }

    private static void AddRolesFromJson(ClaimsIdentity identity, ClaimsPrincipal principal, string json,
        string rolesPropertyName)
    {
        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty(rolesPropertyName, out var rolesElement))
        {
            return;
        }

        foreach (var role in rolesElement.EnumerateArray())
        {
            var roleName = role.GetString();
            if (roleName != null && !principal.HasClaim(ClaimTypes.Role, roleName))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
            }
        }
    }

    private static void AddClientRolesFromJson(ClaimsIdentity identity, ClaimsPrincipal principal, string json,
        string clientId)
    {
        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty(clientId, out var clientElement))
        {
            return;
        }

        if (clientElement.TryGetProperty("roles", out var rolesElement))
        {
            foreach (var role in rolesElement.EnumerateArray())
            {
                var roleName = role.GetString();
                if (roleName != null && !principal.HasClaim(ClaimTypes.Role, roleName))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                }
            }
        }
    }
}