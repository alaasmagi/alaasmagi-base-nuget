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
public interface IBaseEventEnvelope<TContent, TTimestamp>
{
    /// <summary>
    /// Gets and/or initializes the unique message identifier (UUID). It is the idempotency key consumers
    /// use to detect and drop redeliveries.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets and/or initializes the publisher of the message: a single-segment lowercase slug that matches
    /// the publisher's broker credentials (for example <c>identity</c>).
    /// </summary>
    public string Source { get; init; }

    /// <summary>
    /// Gets and/or initializes the subject the message concerns: a single segment equal to the Keycloak
    /// realm name. Equals <see cref="Source"/> for an app publishing its own messages; only the identity
    /// provider publishes on behalf of a tenant it is not.
    /// </summary>
    public string Tenant { get; init; }

    /// <summary>
    /// Gets and/or initializes what happened or what to send: a single hyphenated segment
    /// (for example <c>user-created</c>, <c>user-2fa-otp</c>).
    /// </summary>
    public string Action { get; init; }

    /// <summary>
    /// Gets and/or initializes the timestamp at which the message was published (ISO 8601, UTC).
    /// </summary>
    public TTimestamp Timestamp { get; init; }

    /// <summary>
    /// Gets and/or initializes the schema version of <see cref="Content"/> as a <c>"major.minor"</c> string.
    /// Compatibility is decided on the major component only; consumers ignore unknown fields.
    /// </summary>
    public string ContentVersion { get; init; }

    /// <summary>
    /// Gets and/or initializes the payload associated with the event. Its shape is defined per
    /// <see cref="Action"/> and is flat with no nesting wrapper.
    /// </summary>
    public TContent Content { get; init; }
}
