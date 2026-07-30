using System.Text.Json.Serialization;
using Base.Contracts.Message;

namespace Base.Message;

/// <summary>
/// Provides a base event envelope implementation that carries a strongly typed content payload and
/// uses <see cref="DateTime"/> timestamps.
/// </summary>
/// <typeparam name="TContent">The content payload type carried by the event envelope.</typeparam>
public class BaseEventEnvelope<TContent> : BaseEventEnvelope<TContent, DateTime>, IBaseEventEnvelope<TContent>
{
}

/// <summary>
/// Provides a base event envelope implementation that carries a strongly typed content payload with a
/// strongly typed timestamp.
/// </summary>
/// <typeparam name="TContent">The content payload type carried by the event envelope.</typeparam>
/// <typeparam name="TTimestamp">The timestamp type used by the event envelope.</typeparam>
public class BaseEventEnvelope<TContent, TTimestamp> : IBaseEventEnvelope<TContent, TTimestamp>

{
    /// <summary>
    /// Gets and/or initializes the unique message identifier (UUID) used as the idempotency key.
    /// </summary>
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    /// <summary>
    /// Gets and/or initializes the publisher of the message (single-segment lowercase slug).
    /// </summary>
    [JsonPropertyName("source")]
    public required string Source { get; init; }

    /// <summary>
    /// Gets and/or initializes the subject the message concerns (the Keycloak realm name).
    /// </summary>
    [JsonPropertyName("tenant")]
    public required string Tenant { get; init; }

    /// <summary>
    /// Gets and/or initializes what happened or what to send (single hyphenated segment).
    /// </summary>
    [JsonPropertyName("action")]
    public required string Action { get; init; }

    /// <summary>
    /// Gets and/or initializes the timestamp associated with the event.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public required TTimestamp Timestamp { get; init; }

    /// <summary>
    /// Gets and/or initializes the schema version of the content payload as a <c>"major.minor"</c> string.
    /// </summary>
    [JsonPropertyName("contentVersion")]
    public required string ContentVersion { get; init; }

    /// <summary>
    /// Gets and/or initializes the content payload associated with the event.
    /// </summary>
    [JsonPropertyName("content")]
    public required TContent Content { get; init; }
}
