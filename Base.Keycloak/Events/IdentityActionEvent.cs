using System.Text.Json.Serialization;
using Base.Message;

namespace Base.Keycloak.Events;

public class IdentityActionEvent<TPayload> : BaseEventEnvelope
{
    [JsonPropertyName("realmName")]
    public required string RealmName { get; init; }

    [JsonPropertyName("payload")]
    public required TPayload Payload { get; init; }
}