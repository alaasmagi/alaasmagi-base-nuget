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
    /// Gets and/or initializes the event type identifier used to classify the message.
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    /// Gets and/or initializes the event source identifier used to define the source service of the message.
    /// </summary>
    [JsonPropertyName("source")]
    public required string Source { get; init; }
    
    /// <summary>
    /// Gets and/or initializes the event type identifier used classify the goal the message.
    /// </summary>
    [JsonPropertyName("action")]
    public required string Action { get; init; }
    
    /// <summary>
    /// Gets and/or initializes the timestamp associated with the event.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public required TTimestamp Timestamp { get; init; }
    
    /// <summary>
    /// Gets and/or initializes the content payload associated with the event.
    /// </summary>
    [JsonPropertyName("content")]
    public required TContent Content { get; init; }
}
