namespace Base.Contracts.Message;

/// <summary>
/// Defines a dispatcher contract for routing event envelopes with a strongly typed content payload
/// to their registered handlers.
/// </summary>
/// <typeparam name="TContent">The content payload type carried by the dispatched event envelope.</typeparam>
public interface IBaseEventDispatcher<TContent>
{
    /// <summary>
    /// Dispatches the specified event envelope to its handler.
    /// </summary>
    /// <param name="envelope">The event envelope to dispatch.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous dispatch operation.</returns>
    Task DispatchAsync(IBaseEventEnvelope<TContent> envelope, CancellationToken cancellationToken = default);
}