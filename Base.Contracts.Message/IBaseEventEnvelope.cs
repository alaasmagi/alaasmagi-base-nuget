namespace Base.Contracts.Message;

/// <summary>
/// Defines a base event envelope contract that uses <see cref="DateTime"/> timestamps.
/// </summary>
public interface IBaseEventEnvelope<TContent> : IBaseEventEnvelope<TContent, DateTime>
{
}

/// <summary>
/// Defines a base event envelope contract with a strongly typed timestamp.
/// </summary>
/// <typeparam name="TTimestamp">The timestamp type used by the event envelope.</typeparam>
/// <typeparam name="TContent">The payload type used by the event envelope.</typeparam>
public interface IBaseEventEnvelope<TContent,TTimestamp>
{
    /// <summary>
    /// Gets and/or initializes the event type identifier used to classify the message.
    /// </summary>
    public string Type { get; init; }

    /// <summary>
    /// Gets and/or initializes the event source identifier used to define the source service of the message.
    /// </summary>
    public string Source { get; init; }
    
    /// <summary>
    /// Gets and/or initializes the event type identifier used classify the goal the message.
    /// </summary>
    public string Action { get; init; }
    
    /// <summary>
    /// Gets and/or initializes the timestamp associated with the event.
    /// </summary>
    public TTimestamp Timestamp { get; init; }
    
    /// <summary>
    /// Gets and/or initializes the payload associated with the event.
    /// </summary>
    public TContent Content { get; init; }
}
