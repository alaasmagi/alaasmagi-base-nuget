namespace Base.Contracts.Message;

/// <summary>
/// Defines a handler contract for processing event envelopes with a strongly typed content payload.
/// </summary>
/// <typeparam name="TContent">The content payload type carried by the handled event envelope.</typeparam>
public interface IBaseEventHandler<TContent>
{
    /// <summary>
    /// Handles the specified event envelope.
    /// </summary>
    /// <param name="event">The event envelope to process.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous handling operation.</returns>
    public Task HandleAsync(IBaseEventEnvelope<TContent> @event, CancellationToken cancellationToken = default);
}
