namespace Base.Contracts.Message;

/// <summary>
/// Defines a publisher contract for sending typed event envelopes.
/// </summary>
public interface IBaseEventPublisher
{
    /// <summary>
    /// Publishes an event envelope to the specified topic.
    /// </summary>
    /// <param name="topic">The topic or routing key used to publish the event.</param>
    /// <param name="message">The event envelope to publish.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <typeparam name="TEvent">The event envelope type being published.</typeparam>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    Task PublishAsync<TEvent>(string topic, TEvent message, CancellationToken cancellationToken = default) 
        where TEvent : IBaseEventEnvelope;
}
