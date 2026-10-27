namespace Base.Contracts.Message;

/// <summary>
/// Represents one raw message delivered from a queue, before its body is deserialized. Consumers that need
/// transport metadata (for example the routing key, to verify that the key and the body agree) receive this
/// instead of an already deserialized envelope.
/// </summary>
public sealed record MessageDelivery
{
    /// <summary>
    /// Gets the routing key the message was published with (<c>{source}.{tenant}.{action}</c>).
    /// </summary>
    public required string RoutingKey { get; init; }

    /// <summary>
    /// Gets the raw message body (UTF-8 JSON of the event envelope).
    /// </summary>
    public required ReadOnlyMemory<byte> Body { get; init; }

    /// <summary>
    /// Gets a value indicating whether the broker has delivered this message before.
    /// </summary>
    public bool Redelivered { get; init; }
}
