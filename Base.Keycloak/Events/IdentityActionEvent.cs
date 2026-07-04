using System.Text.Json.Serialization;
using Base.Message;

namespace Base.Keycloak.Events;

/// <summary>
/// Represents an identity action event with a typed payload.
/// </summary>
/// <typeparam name="TPayload">The payload type carried by the identity action event.</typeparam>
public class IdentityActionEvent<TPayload> : BaseEventEnvelope
{
    /// <summary>
    /// Gets the Keycloak realm name associated with the event.
    /// </summary>
    [JsonPropertyName("realmName")]
    public required string RealmName { get; init; }

    /// <summary>
    /// Gets the typed payload associated with the identity action.
    /// </summary>
    [JsonPropertyName("payload")]
    public required TPayload Payload { get; init; }
}
