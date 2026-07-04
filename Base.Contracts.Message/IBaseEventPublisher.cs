namespace Base.Contracts.Message;

public interface IBaseEventPublisher
{
    Task PublishAsync<TEvent>(string topic, TEvent message, CancellationToken cancellationToken = default) 
        where TEvent : IBaseEventEnvelope;
}