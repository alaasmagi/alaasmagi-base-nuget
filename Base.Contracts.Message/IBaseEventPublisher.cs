namespace Base.Contracts.Message;

/// <summary>
/// Defines a publisher contract for sending event envelopes with a strongly typed content payload.
/// </summary>
public interface IBaseEventPublisher
{
    /// <summary>
    /// Publishes an event envelope to the specified topic.
    /// </summary>
    /// <param name="topic">The topic or routing key used to publish the event.</param>
    /// <param name="message">The event envelope to publish.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <typeparam name="TContent">The content payload type carried by the event envelope.</typeparam>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    public Task PublishAsync<TContent>(string topic, IBaseEventEnvelope<TContent> message,
        CancellationToken cancellationToken = default);
}
