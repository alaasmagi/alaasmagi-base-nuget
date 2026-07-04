namespace Base.Contracts.Message;

public interface IBaseEventHandler<TEvent> where TEvent : IBaseEventEnvelope
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}