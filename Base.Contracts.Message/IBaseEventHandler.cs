namespace Base.Contracts.Message;

/// <summary>
/// Defines a handler contract for processing typed event envelopes.
/// </summary>
/// <typeparam name="TEvent">The event envelope type handled by the implementation.</typeparam>
public interface IBaseEventHandler<TEvent> where TEvent : IBaseEventEnvelope
{
    /// <summary>
    /// Handles the specified event envelope.
    /// </summary>
    /// <param name="event">The event envelope to process.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous handling operation.</returns>
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}
