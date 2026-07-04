using System.Text.Json.Serialization;
using Base.Contracts.Message;

namespace Base.Message;

public abstract class BaseEventEnvelope : BaseEventEnvelope<DateTime>
{
}

public abstract class BaseEventEnvelope<TTimestamp> : IBaseEventEnvelope<TTimestamp>
{
    [JsonPropertyName("eventType")]
    public required string EventType { get; init; }
    [JsonPropertyName("timestamp")]
    public required TTimestamp Timestamp { get; init; }
}