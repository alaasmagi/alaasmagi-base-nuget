using System.Text.Json.Serialization;
using Base.Message;

namespace Base.Keycloak.Events;

/// <summary>
/// Represents a Keycloak identity lifecycle event.
/// </summary>
public class IdentityLifecycleEvent : BaseEventEnvelope
{
    /// <summary>
    /// Gets the Keycloak realm name associated with the event.
    /// </summary>
    [JsonPropertyName("realmName")]
    public required string RealmName { get; init; }

    /// <summary>
    /// Gets the Keycloak user identifier associated with the event.
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; init; }

    /// <summary>
    /// Gets the optional reason associated with a user ban or disable action.
    /// </summary>
    [JsonPropertyName("banReason")]
    public string? BanReason { get; init; }

    /// <summary>
    /// Gets the raw representation payload supplied by Keycloak when available.
    /// </summary>
    [JsonPropertyName("representation")]
    public string? Representation { get; init; }
}
