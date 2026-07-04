namespace Base.Contracts.Message;

public interface IBaseEventEnvelope : IBaseEventEnvelope<DateTime>
{
}

public interface IBaseEventEnvelope<TTimestamp>
{
    string EventType { get; }
    TTimestamp Timestamp { get; }
}