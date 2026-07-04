using System.Text.Json.Serialization;
using Base.Message;

namespace Base.Keycloak.Events;

public class IdentityLifecycleEvent : BaseEventEnvelope
{
    [JsonPropertyName("realmName")]
    public required string RealmName { get; init; }

    [JsonPropertyName("userId")]
    public string? UserId { get; init; }

    [JsonPropertyName("banReason")]
    public string? BanReason { get; init; }

    [JsonPropertyName("representation")]
    public string? Representation { get; init; }
}