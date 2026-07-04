using System.Text.Json.Serialization;
using Base.Contracts.Message;

namespace Base.Message;

/// <summary>
/// Provides a base event envelope implementation that uses <see cref="DateTime"/> timestamps.
/// </summary>
public abstract class BaseEventEnvelope : BaseEventEnvelope<DateTime>
{
}

/// <summary>
/// Provides a base event envelope implementation with a strongly typed timestamp.
/// </summary>
/// <typeparam name="TTimestamp">The timestamp type used by the event envelope.</typeparam>
public abstract class BaseEventEnvelope<TTimestamp> : IBaseEventEnvelope<TTimestamp>
{
    /// <summary>
    /// Gets the event type identifier used to route or classify the message.
    /// </summary>
    [JsonPropertyName("eventType")]
    public required string EventType { get; init; }

    /// <summary>
    /// Gets the timestamp associated with the event.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public required TTimestamp Timestamp { get; init; }
}
