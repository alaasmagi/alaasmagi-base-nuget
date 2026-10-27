using System.Text;
using System.Text.Json;
using Base.Contracts.Message;
using Microsoft.Extensions.Logging;

namespace Base.Message.RabbitMQ;

/// <summary>
/// Provides a hosted RabbitMQ listener that consumes event envelopes with a strongly typed content payload from a queue.
/// A message is acked once the handler returns and rejected without requeue if deserialization or the handler throws.
/// </summary>
/// <typeparam name="TContent">The content payload type carried by the consumed event envelopes.</typeparam>
public abstract class RabbitMqConsumerBase<TContent> : RabbitMqDeliveryConsumerBase
{
    private readonly IBaseEventHandler<TContent> _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConsumerBase{TContent}"/> class. The queue and its
    /// bindings are part of the externally managed topology and must exist before the consumer starts; this
    /// class only consumes from <paramref name="queueName"/>.
    /// </summary>
    protected RabbitMqConsumerBase(
        RabbitMqConnectionManager connectionManager,
        IBaseEventHandler<TContent> handler,
        ILogger logger,
        string queueName)
        : base(connectionManager, logger, queueName)
    {
        _handler = handler;
    }

    /// <summary>
    /// Deserializes the delivery into a <see cref="BaseEventEnvelope{TContent}"/> and passes it to the registered handler.
    /// </summary>
    protected override async Task<MessageDisposition> HandleDeliveryAsync(
        MessageDelivery delivery,
        CancellationToken cancellationToken)
    {
        var json = Encoding.UTF8.GetString(delivery.Body.Span);
        var @event = JsonSerializer.Deserialize<BaseEventEnvelope<TContent>>(json);

        if (@event != null)
        {
            await _handler.HandleAsync(@event, cancellationToken);
        }

        return MessageDisposition.Ack;
    }
}
