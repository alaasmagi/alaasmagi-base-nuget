namespace Base.Contracts.Message;

/// <summary>
/// Defines a publisher contract for sending event envelopes with a strongly typed content payload.
/// </summary>
public interface IBaseEventPublisher
{
    /// <summary>
    /// Publishes an event envelope. The routing key is derived from the envelope itself; callers do not
    /// supply one. The call completes only after the broker has confirmed the message (or the attempt has
    /// failed), and the outcome is reported through the returned <see cref="PublishResult"/>.
    /// </summary>
    /// <param name="message">The event envelope to publish.</param>
    /// <param name="expiration">
    /// Optional per-message time-to-live in milliseconds (the AMQP <c>expiration</c> property), unset by
    /// default. Callers use it for time-limited messages; the package never decides this for them.
    /// </param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <typeparam name="TContent">The content payload type carried by the event envelope.</typeparam>
    /// <returns>A task resolving to the publish outcome.</returns>
    public Task<PublishResult> PublishAsync<TContent>(
        IBaseEventEnvelope<TContent> message,
        string? expiration = null,
        CancellationToken cancellationToken = default);
}
