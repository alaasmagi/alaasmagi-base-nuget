namespace Base.Message;

/// <summary>A notification whose payload and envelope share the same expiry.</summary>
public interface IExpiringNotificationContent
{
    DateTimeOffset ExpiresAt { get; }
}
