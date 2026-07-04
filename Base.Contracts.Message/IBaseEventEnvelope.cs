namespace Base.Contracts.Message;

/// <summary>
/// Defines a base event envelope contract that uses <see cref="DateTime"/> timestamps.
/// </summary>
public interface IBaseEventEnvelope : IBaseEventEnvelope<DateTime>
{
}

/// <summary>
/// Defines a base event envelope contract with a strongly typed timestamp.
/// </summary>
/// <typeparam name="TTimestamp">The timestamp type used by the event envelope.</typeparam>
public interface IBaseEventEnvelope<TTimestamp>
{
    /// <summary>
    /// Gets the event type identifier used to route or classify the message.
    /// </summary>
    string EventType { get; }

    /// <summary>
    /// Gets the timestamp associated with the event.
    /// </summary>
    TTimestamp Timestamp { get; }
}
